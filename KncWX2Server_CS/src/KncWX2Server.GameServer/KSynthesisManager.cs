using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLSynthesisManager.{h,cpp} and Common/X2Data/XSLUnit.h (EQIP_POSITION):
/// manages avatar synthesis groups, equip-position offset mapping
/// (WeaponHand=2 -> -2=0, DefenceBody=8..Foot=11 -> -7=1..4, DefenceHair=6 -> -1=5),
/// socket option group mapping, and weighted lottery item generation.
/// </summary>
public sealed class KSynthesisManager
{
    public const int EquipNone = 0;
    public const int EquipQuickSlot = 1;
    public const int EquipWeaponHand = 2;
    public const int EquipDefenceHair = 6;
    public const int EquipDefenceFace = 7;
    public const int EquipDefenceBody = 8;
    public const int EquipDefenceLeg = 9;
    public const int EquipDefenceHand = 10;
    public const int EquipDefenceFoot = 11;

    private readonly Dictionary<int, int> _avatarSocketOptionGroups = new();
    private readonly Dictionary<int, KLottery> _itemGroups = new();

    public bool AddAvatarSocketOptionForEquipPosition(int equipPosition, int socketGroupId)
    {
        if (equipPosition is not (EquipWeaponHand or EquipDefenceHair or EquipDefenceFace
            or EquipDefenceBody or EquipDefenceLeg or EquipDefenceHand or EquipDefenceFoot))
            return false;

        _avatarSocketOptionGroups[equipPosition] = socketGroupId;
        return true;
    }

    public int GetAvatarSocketOptionGroupId(int equipPosition)
        => _avatarSocketOptionGroups.GetValueOrDefault(equipPosition, 0);

    public Dictionary<int, int> GetAvatarSocketOptionGroups()
        => new(_avatarSocketOptionGroups);

    public bool AddSynthesisGroup(int groupId, int itemId, float rate)
    {
        if (groupId <= 0 || itemId <= 0) return false;
        if (!_itemGroups.TryGetValue(groupId, out var lot))
        {
            lot = new KLottery();
            _itemGroups[groupId] = lot;
        }
        return lot.AddCase(itemId, rate);
    }

    public static int ComputeGroupId(int unitType, int equipPosition)
    {
        int adjusted = equipPosition switch
        {
            EquipWeaponHand => equipPosition - 2,
            EquipDefenceBody => equipPosition - 7,
            EquipDefenceLeg => equipPosition - 7,
            EquipDefenceHand => equipPosition - 7,
            EquipDefenceFoot => equipPosition - 7,
            EquipDefenceHair => equipPosition - 1,
            _ => equipPosition,
        };
        return (unitType * 10) + adjusted;
    }

    public (int ok, int resultItemId, KItemInfo? resultItem) GetSynthesisResultItem(
        int unitType,
        int equipPosition,
        int socketNo,
        double? fixedRoll = null)
    {
        int groupId = ComputeGroupId(unitType, equipPosition);
        if (groupId == 0 || !_itemGroups.TryGetValue(groupId, out var lot))
            return (NetError.ERR_RANDOM_ITEM_02, 0, null);

        int itemId = lot.Decision(fixedRoll);
        if (itemId == KLottery.CaseBlank || itemId <= 0)
            return (NetError.ERR_RANDOM_ITEM_02, 0, null);

        var item = new KItemInfo
        {
            ItemId = itemId,
            Quantity = 1,
            ItemSockets = [socketNo],
        };
        return (NetError.NET_OK, itemId, item);
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pSynthesisManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddAvatarSocketOptionForEquipPosition"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddAvatarSocketOptionForEquipPosition(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
            t["AddSynthesisGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddSynthesisGroup(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgFloat(a, o + 2));
                return DynValue.Nil;
            });
        });
    }
}
