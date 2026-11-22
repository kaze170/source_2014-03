using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/KAttribNpcTable.{h,cpp}:
/// elemental/champion monster attribute tables keyed by (NpcLevel, Difficulty) using KLottery,
/// sub-stage counts, NPC drop counts, no-drop NPC list, and MoonSharp Lua loading.
/// </summary>
public sealed class KAttribNpcTable
{
    public enum AttribType
    {
        AtNone = 0,
        AtBlaze = 1,
        AtFrozen = 2,
        AtPoison = 3,
        AtPiercing = 4,
        AtShock = 5,
        AtSnatch = 6,
        AtBlazeMaster = 7,
        AtFrozenMaster = 8,
        AtPoisonMaster = 9,
        AtMasterPiercing = 10,
        AtMasterShock = 11,
        AtMasterSnatch = 12,
    }

    private readonly Dictionary<(int npcLv, int difficulty), KLottery> _tables = new();
    private readonly Dictionary<(int dungeonType, int difficulty), int> _attribSubStageCounts = new();
    private readonly Dictionary<(int dungeonType, int playerCount), int> _npcDropCounts = new();
    private readonly HashSet<int> _noDropNpcs = new();
    private readonly Dictionary<int, (int g1, int g2, int g3)> _attribDropInfos = new();
    private readonly Dictionary<int, KLottery> _attribDropGroups = new();

    public IReadOnlyDictionary<(int dungeonType, int difficulty), int> AttribSubStageCounts => _attribSubStageCounts;
    public IReadOnlyDictionary<(int dungeonType, int playerCount), int> NpcDropCounts => _npcDropCounts;
    public IReadOnlySet<int> NoDropNpcs => _noDropNpcs;
    public IReadOnlyDictionary<int, (int g1, int g2, int g3)> AttribDropInfos => _attribDropInfos;
    public IReadOnlyDictionary<int, KLottery> AttribDropGroups => _attribDropGroups;
    public int LoadedNpcRuleCount { get; private set; }

    public KAttribNpcTable()
    {
        SeedDefaults();
    }

    public void AddAttribNpcRate(int npcLevel, int difficulty, AttribType type, float prob)
    {
        var key = (npcLevel, difficulty);
        if (!_tables.TryGetValue(key, out var lottery))
        {
            lottery = new KLottery();
            _tables[key] = lottery;
        }
        lottery.AddCase((int)type, prob);
    }

    public AttribType RollNpcAttrib(int npcLevel, int difficulty, double? fixedUnitRoll = null)
    {
        if (!_tables.TryGetValue((npcLevel, difficulty), out var lottery))
        {
            // Fallback to difficulty bucket if exact level isn't registered
            if (!_tables.TryGetValue((0, difficulty), out lottery))
                return AttribType.AtNone;
        }
        int res = lottery.Decision(fixedUnitRoll);
        return res == KLottery.CaseBlank ? AttribType.AtNone : (AttribType)res;
    }

    private void SeedDefaults()
    {
        // Normal (0), Hard (1), VeryHard (2)
        for (int diff = 0; diff <= 2; diff++)
        {
            float baseRate = 2.0f + diff * 2.5f;
            AddAttribNpcRate(0, diff, AttribType.AtBlaze, baseRate);
            AddAttribNpcRate(0, diff, AttribType.AtFrozen, baseRate);
            AddAttribNpcRate(0, diff, AttribType.AtPoison, baseRate);
            AddAttribNpcRate(0, diff, AttribType.AtPiercing, baseRate);
            AddAttribNpcRate(0, diff, AttribType.AtShock, baseRate);
            AddAttribNpcRate(0, diff, AttribType.AtSnatch, baseRate);
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pAttribNpcManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetAttribSubStageCount"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _attribSubStageCounts[(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1))] = KScriptManager.ArgInt(a, o + 2);
                return DynValue.Nil;
            });
            t["SetNpcDropCount"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _npcDropCounts[(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1))] = KScriptManager.ArgInt(a, o + 2);
                return DynValue.Nil;
            });
            t["AddNoDropNpc"] = DynValue.NewCallback((_, a) =>
            {
                int npcId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (npcId > 0) _noDropNpcs.Add(npcId);
                return DynValue.Nil;
            });
            t["AddAttribDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int abilityId = KScriptManager.ArgInt(a, o);
                _attribDropInfos[abilityId] = (KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3));
                return DynValue.Nil;
            });
            t["AddAttribDropGroupInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int groupId = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                float prob = KScriptManager.ArgFloat(a, o + 2);
                if (!_attribDropGroups.TryGetValue(groupId, out var lot))
                {
                    lot = new KLottery();
                    _attribDropGroups[groupId] = lot;
                }
                if (itemId > 0 && prob > 0) lot.AddCase(itemId, prob);
                return DynValue.Nil;
            });
            t["AddAttribNpcInfo_LUA"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl == null) return DynValue.Nil;
                int diff = (int)(tbl.Get("m_eDifficulty").CastToNumber() ?? 0);
                int lvBegin = (int)(tbl.Get("m_iNpcLvBegin").CastToNumber() ?? 1);
                int lvEnd = (int)(tbl.Get("m_iNpcLvEnd").CastToNumber() ?? lvBegin);
                float blaze = (float)(tbl.Get("m_fBlazeRate").CastToNumber() ?? 0);
                float frozen = (float)(tbl.Get("m_fFrozenRate").CastToNumber() ?? 0);
                float poison = (float)(tbl.Get("m_fPoisonRate").CastToNumber() ?? 0);
                float piercing = (float)(tbl.Get("m_fPiercingRate").CastToNumber() ?? 0);
                float shock = (float)(tbl.Get("m_fShockRate").CastToNumber() ?? 0);
                float snatch = (float)(tbl.Get("m_fSnatchRate").CastToNumber() ?? 0);
                for (int lv = lvBegin; lv <= lvEnd; lv++)
                {
                    if (blaze > 0) AddAttribNpcRate(lv, diff, AttribType.AtBlaze, blaze);
                    if (frozen > 0) AddAttribNpcRate(lv, diff, AttribType.AtFrozen, frozen);
                    if (poison > 0) AddAttribNpcRate(lv, diff, AttribType.AtPoison, poison);
                    if (piercing > 0) AddAttribNpcRate(lv, diff, AttribType.AtPiercing, piercing);
                    if (shock > 0) AddAttribNpcRate(lv, diff, AttribType.AtShock, shock);
                    if (snatch > 0) AddAttribNpcRate(lv, diff, AttribType.AtSnatch, snatch);
                }
                LoadedNpcRuleCount++;
                return DynValue.Nil;
            });
        });
    }
}
