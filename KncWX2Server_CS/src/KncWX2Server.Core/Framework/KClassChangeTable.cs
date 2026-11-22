using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/ClassChangeTable.{h,cpp} (SERV_UNIT_CLASS_CHANGE_ITEM):
/// class-change sibling groups (1st/2nd job paths) and cross-class skill/memo/item/quest remapping.
/// </summary>
public sealed class KClassChangeTable
{
    public static KClassChangeTable Instance { get; } = new();

    private readonly Dictionary<int, int> _classGroupByClass = new();
    private readonly Dictionary<int, List<int>> _classesByGroup = new();
    private readonly Dictionary<int, int> _skillGroupBySkill = new();
    private readonly Dictionary<(int groupId, int unitClass), int> _skillByGroupAndClass = new();
    private readonly Dictionary<int, int> _memoGroupByMemo = new();
    private readonly Dictionary<(int groupId, int unitClass), int> _memoByGroupAndClass = new();
    private readonly Dictionary<int, int> _itemGroupByItem = new();
    private readonly Dictionary<(int groupId, int unitClass), int> _itemByGroupAndClass = new();
    private readonly Dictionary<int, int> _questGroupByQuest = new();
    private readonly Dictionary<(int groupId, int unitClass), int> _questByGroupAndClass = new();
    private readonly HashSet<int> _deleteQuests = [];

    public KClassChangeTable()
    {
        SeedDefaultTwGroups();
    }

    public void SetClassGroupId(int classGroup, int unitClass)
    {
        _classGroupByClass[unitClass] = classGroup;
        if (!_classesByGroup.TryGetValue(classGroup, out var list))
        {
            list = [];
            _classesByGroup[classGroup] = list;
        }
        if (!list.Contains(unitClass)) list.Add(unitClass);
    }

    public void SetClassChangeSkill(int skillGroup, int skillId, int unitClass)
    {
        _skillGroupBySkill[skillId] = skillGroup;
        _skillByGroupAndClass[(skillGroup, unitClass)] = skillId;
    }

    public void SetClassChangeMemo(int memoGroup, int memoId, int unitClass)
    {
        _memoGroupByMemo[memoId] = memoGroup;
        _memoByGroupAndClass[(memoGroup, unitClass)] = memoId;
    }

    public void SetClassChangeInventoryItem(int itemGroup, int itemId, int unitClass)
    {
        _itemGroupByItem[itemId] = itemGroup;
        _itemByGroupAndClass[(itemGroup, unitClass)] = itemId;
    }

    public void SetClassChangeQuest(int questGroup, int questId, int unitClass)
    {
        _questGroupByQuest[questId] = questGroup;
        _questByGroupAndClass[(questGroup, unitClass)] = questId;
    }

    public void AddDeleteQuest(int questId) => _deleteQuests.Add(questId);

    public bool GetClassGroupId(int unitClass, out int classGroupId)
        => _classGroupByClass.TryGetValue(unitClass, out classGroupId);

    public IReadOnlyList<int> GetUnitClassList(int classGroupId)
        => _classesByGroup.TryGetValue(classGroupId, out var list) ? list : [];

    public bool CompareUnitClass(int nowUnitClass, int newUnitClass)
        => nowUnitClass != newUnitClass &&
           _classGroupByClass.TryGetValue(nowUnitClass, out int g1) &&
           _classGroupByClass.TryGetValue(newUnitClass, out int g2) &&
           g1 == g2;

    public bool AreInSameClassGroup(int nowUnitClass, int newUnitClass)
        => CompareUnitClass(nowUnitClass, newUnitClass);

    public bool TryGetNewSkill(int newUnitClass, int oldSkillId, out int newSkillId)
    {
        newSkillId = 0;
        return _skillGroupBySkill.TryGetValue(oldSkillId, out int grp) &&
               _skillByGroupAndClass.TryGetValue((grp, newUnitClass), out newSkillId);
    }

    public bool TryGetNewMemo(int newUnitClass, int oldMemoId, out int newMemoId)
    {
        newMemoId = 0;
        return _memoGroupByMemo.TryGetValue(oldMemoId, out int grp) &&
               _memoByGroupAndClass.TryGetValue((grp, newUnitClass), out newMemoId);
    }

    public bool TryGetNewItem(int newUnitClass, int oldItemId, out int newItemId)
    {
        newItemId = 0;
        return _itemGroupByItem.TryGetValue(oldItemId, out int grp) &&
               _itemByGroupAndClass.TryGetValue((grp, newUnitClass), out newItemId);
    }

    public bool TryGetNewQuest(int newUnitClass, int oldQuestId, out int newQuestId)
    {
        newQuestId = 0;
        return _questGroupByQuest.TryGetValue(oldQuestId, out int grp) &&
               _questByGroupAndClass.TryGetValue((grp, newUnitClass), out newQuestId);
    }

    public IReadOnlySet<int> DeleteQuests => _deleteQuests;

    private void SeedDefaultTwGroups()
    {
        // Exact XSLUnit.h UNIT_CLASS IDs: Elsword (11/12), Aisha (21/22), Rena (31/32), Raven (41/42), Eve (51/52), Chung (61/62), Ara (71/72), Elesis (81/82)
        SetClassGroupId(11, 10); SetClassGroupId(11, 11); SetClassGroupId(11, 22);
        SetClassGroupId(12, 100); SetClassGroupId(12, 101); SetClassGroupId(12, 112);
        SetClassGroupId(21, 14); SetClassGroupId(21, 15); SetClassGroupId(21, 23);
        SetClassGroupId(22, 104); SetClassGroupId(22, 105); SetClassGroupId(22, 113);
        SetClassGroupId(31, 12); SetClassGroupId(31, 13); SetClassGroupId(31, 24);
        SetClassGroupId(32, 102); SetClassGroupId(32, 103); SetClassGroupId(32, 114);
        SetClassGroupId(41, 16); SetClassGroupId(41, 17); SetClassGroupId(41, 25);
        SetClassGroupId(42, 106); SetClassGroupId(42, 107); SetClassGroupId(42, 115);
        SetClassGroupId(51, 18); SetClassGroupId(51, 19); SetClassGroupId(51, 26);
        SetClassGroupId(52, 108); SetClassGroupId(52, 109); SetClassGroupId(52, 116);
        SetClassGroupId(61, 20); SetClassGroupId(61, 21); SetClassGroupId(61, 27);
        SetClassGroupId(62, 110); SetClassGroupId(62, 111); SetClassGroupId(62, 117);
        SetClassGroupId(71, 28); SetClassGroupId(71, 31);
        SetClassGroupId(72, 118); SetClassGroupId(72, 119);
        SetClassGroupId(81, 29); SetClassGroupId(81, 30);
        SetClassGroupId(82, 120); SetClassGroupId(82, 121);
    }

    public void BindToLua(KScriptManager script, string globalName = "ClassChangeTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetClassGroupID"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetClassGroupId(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
            t["SetClassChange_Skill"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetClassChangeSkill(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["SetClassChange_Memo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetClassChangeMemo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["SetClassChange_InventoryItem"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetClassChangeInventoryItem(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["SetClassChange_Item"] = t["SetClassChange_InventoryItem"];
            t["SetClassChange_Quest"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetClassChangeQuest(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["SetClassChange_DelQuest"] = DynValue.NewCallback((_, a) =>
            {
                AddDeleteQuest(KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!)));
                return DynValue.Nil;
            });
        });
    }
}
