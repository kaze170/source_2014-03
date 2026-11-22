using KncWX2Server.Core.Database;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserSkillTree.{h,cpp}:
/// manages character skill pages, SP budget, learning/leveling skills, and equipping/swapping slots 0..7.
/// </summary>
public sealed class KUserSkillTree
{
    public const int MaxSkillSlots = 8; // Slots 0..3 (A-bar), Slots 4..7 (B-bar)
    public const int MaxSkillPages = 3;
    public const int EdNeededToExpandSecondSkillPage = 500_000;
    public const int SiEtcWsCommonLove = 6001;
    public const int SiEtcWsCommonLoveLegacy = 900;

    public sealed class SkillTempletInfo
    {
        public int SkillId { get; init; }
        public byte RequiredLevel { get; init; } = 1;
        public int BeforeSkillId { get; init; }
        public int MaxLevel { get; init; } = 20;
        public bool IsSealed { get; init; }
        public int NeedSp { get; init; } = 1;
    }

    private readonly Dictionary<int, int> _learnedSkills = new(); // SkillId -> Level
    private readonly int[] _equippedSlots = new int[MaxSkillSlots];
    private readonly Dictionary<int, int> _skillNotePages = new(); // PageNum -> MemoId
    private readonly Dictionary<int, int> _pageSkillPoints = new(); // PageNum -> SP
    private readonly HashSet<int> _unsealedSkills = new();
    private readonly Dictionary<int, SkillTempletInfo> _templets = new();
    public int ActiveSkillPage { get; set; } = 1;
    public int TheNumberOfSkillPagesAvailable { get; private set; } = 1;
    public int AvailableSkillPoints { get; private set; }
    public int AvailableCashSkillPoints { get; private set; }
    public int MaxCashSkillPoints { get; set; }
    public string CashSkillPointEndDate { get; set; } = string.Empty;
    public bool HasSkillNote { get; set; } = true;
    public sbyte SkillNoteMaxPageNum { get; set; } = 3;

    public bool CanExpandSkillPage() => TheNumberOfSkillPagesAvailable < MaxSkillPages;

    public int GetNextSkillPageExpandEd() => TheNumberOfSkillPagesAvailable switch
    {
        1 or 2 => EdNeededToExpandSecondSkillPage,
        _ => -1,
    };

    public (bool ok, int newPageCount, KncWX2Server.Core.Packets.KUserSkillPageData pageData) ExpandSkillPage(
        int unitLevel, sbyte unitClass)
    {
        if (!CanExpandSkillPage())
            return (false, TheNumberOfSkillPagesAvailable, new KncWX2Server.Core.Packets.KUserSkillPageData());

        int newPage = ++TheNumberOfSkillPagesAvailable;
        int initSp = Math.Max(0, (unitLevel - 1) * 2 + 2);
        _pageSkillPoints[newPage] = initSp;
        var pageData = BuildSkillPageData(initSp, AvailableCashSkillPoints);
        return (true, TheNumberOfSkillPagesAvailable, pageData);
    }

    public (bool ok, KncWX2Server.Core.Packets.KUserSkillPageData pageData) SelectSkillPage(int pageNumber, int unitLevel)
    {
        if (pageNumber < 1 || pageNumber > TheNumberOfSkillPagesAvailable)
            return (false, new KncWX2Server.Core.Packets.KUserSkillPageData());

        _pageSkillPoints[ActiveSkillPage] = AvailableSkillPoints;
        ActiveSkillPage = pageNumber;
        if (!_pageSkillPoints.TryGetValue(pageNumber, out int sp))
        {
            sp = Math.Max(0, (unitLevel - 1) * 2 + 2);
            _pageSkillPoints[pageNumber] = sp;
        }
        AvailableSkillPoints = sp;
        return (true, BuildSkillPageData(AvailableSkillPoints, AvailableCashSkillPoints));
    }

    public KncWX2Server.Core.Packets.KUserSkillPageData BuildSkillPageData(int sp, int csp)
    {
        var data = new KncWX2Server.Core.Packets.KUserSkillPageData
        {
            SkillPoint = (ushort)Math.Clamp(sp, 0, ushort.MaxValue),
            CashSkillPoint = (ushort)Math.Clamp(csp, 0, ushort.MaxValue),
        };
        for (int i = 0; i < 4; i++)
        {
            int sidA = _equippedSlots[i];
            data.EquippedSkill[i].SkillId = (short)sidA;
            data.EquippedSkill[i].SkillLevel = (byte)GetSkillLevel(sidA);

            int sidB = _equippedSlots[i + 4];
            data.EquippedSkillSlotB[i].SkillId = (short)sidB;
            data.EquippedSkillSlotB[i].SkillLevel = (byte)GetSkillLevel(sidB);
        }
        foreach (var kv in _learnedSkills)
        {
            data.UserSkillData.Add(new KncWX2Server.Core.Packets.KUserSkillData
            {
                SkillId = (short)kv.Key,
                SkillLevel = (byte)Math.Clamp(kv.Value, 0, 255),
                SkillCsPoint = 0,
            });
        }
        return data;
    }

    public void SetSkillNoteState(bool hasNote, sbyte maxPages, IEnumerable<int> memoIds)
    {
        HasSkillNote = hasNote;
        SkillNoteMaxPageNum = maxPages;
        _skillNotePages.Clear();
        sbyte page = 1;
        foreach (int memoId in memoIds)
        {
            if (memoId > 0)
                _skillNotePages[page++] = memoId;
        }
    }

    public bool IsExistSkillNotePage(sbyte pageNum) => pageNum >= 1 && pageNum <= SkillNoteMaxPageNum;

    public bool IsExistSkillNoteMemoId(int memoId) => memoId > 0 && _skillNotePages.Values.Contains(memoId);

    public void RegisterSkillNoteMemo(sbyte pageNum, int memoId)
    {
        if (pageNum >= 1)
            _skillNotePages[pageNum] = memoId;
    }

    public IReadOnlyDictionary<int, int> GetSkillNotePagesSnapshot() => new Dictionary<int, int>(_skillNotePages);

    public List<int> GetSkillNoteMemoIds() => _skillNotePages.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();

    public bool TryGetExpandSkillNotePage(int level, out sbyte expandedMaxPageNum)
    {
        expandedMaxPageNum = level >= 45 ? (sbyte)6 : level >= 30 ? (sbyte)5 : level >= 20 ? (sbyte)4 : (sbyte)3;
        if (expandedMaxPageNum > SkillNoteMaxPageNum)
        {
            SkillNoteMaxPageNum = expandedMaxPageNum;
            return true;
        }
        return false;
    }

    public void Init(int unitLevel, IReadOnlyDictionary<int, int>? persistedSlots = null)
    {
        _learnedSkills.Clear();
        Array.Fill(_equippedSlots, 0);
        int totalSp = Math.Max(0, (unitLevel - 1) * 2 + 2);
        if (persistedSlots != null)
        {
            foreach (var kv in persistedSlots)
            {
                if (kv.Key >= 0 && kv.Key < MaxSkillSlots)
                    _equippedSlots[kv.Key] = kv.Value;
            }
        }
        AvailableSkillPoints = Math.Max(0, totalSp - _learnedSkills.Values.Sum());
    }

    public void InitSkillPageState(int pageCount, int activePage, IReadOnlyDictionary<int, int>? notePages)
    {
        TheNumberOfSkillPagesAvailable = Math.Clamp(pageCount, 1, MaxSkillPages);
        ActiveSkillPage = Math.Clamp(activePage, 1, TheNumberOfSkillPagesAvailable);
        _skillNotePages.Clear();
        if (notePages != null)
        {
            foreach (var kv in notePages)
            {
                if (kv.Key >= 1)
                    _skillNotePages[kv.Key] = kv.Value;
            }
        }
    }

    public void RegisterSkillTemplet(SkillTempletInfo templet) => _templets[templet.SkillId] = templet;

    public void UnlockSealedSkill(int skillId) => _unsealedSkills.Add(skillId);

    public bool IsSkillUnsealed(int skillId) =>
        !_templets.TryGetValue(skillId, out var t) || !t.IsSealed || _unsealedSkills.Contains(skillId);

    public bool TryLearnSkill(int skillId, int maxLevel = 20, byte unitLevel = 99)
    {
        if (skillId <= 0 || AvailableSkillPoints <= 0) return false;
        int effectiveMax = maxLevel;
        int spCost = 1;
        if (_templets.TryGetValue(skillId, out var tpl))
        {
            if (unitLevel < tpl.RequiredLevel) return false;
            if (tpl.IsSealed && !_unsealedSkills.Contains(skillId)) return false;
            if (tpl.BeforeSkillId > 0 && GetSkillLevel(tpl.BeforeSkillId) <= 0) return false;
            effectiveMax = tpl.MaxLevel;
            spCost = Math.Max(1, tpl.NeedSp);
        }
        if (AvailableSkillPoints < spCost) return false;
        int cur = _learnedSkills.GetValueOrDefault(skillId, 0);
        if (cur >= effectiveMax) return false;
        _learnedSkills[skillId] = cur + 1;
        AvailableSkillPoints -= spCost;
        return true;
    }

    public int ResetSingleSkill(int skillId)
    {
        if (!_learnedSkills.TryGetValue(skillId, out int lvl) || lvl <= 0) return 0;
        int spCost = _templets.TryGetValue(skillId, out var t) ? Math.Max(1, t.NeedSp) : 1;
        int refund = lvl * spCost;
        _learnedSkills.Remove(skillId);
        for (int i = 0; i < MaxSkillSlots; i++)
            if (_equippedSlots[i] == skillId) _equippedSlots[i] = 0;
        AvailableSkillPoints += refund;
        return refund;
    }

    public int ResetAllSkills(SqliteDatabaseManager db, long unitUid, int unitLevel, bool resetUnsealed = false, bool resetCashSkillPoint = false, bool resetSkillNote = false)
    {
        ResetSkillPage(db, unitUid, unitLevel);
        if (resetUnsealed) _unsealedSkills.Clear();
        if (resetCashSkillPoint) { AvailableCashSkillPoints = 0; MaxCashSkillPoints = 0; CashSkillPointEndDate = string.Empty; }
        if (resetSkillNote) { SkillNoteMaxPageNum = 0; _skillNotePages.Clear(); }
        return AvailableSkillPoints;
    }

    public int GetSkillLevel(int skillId) => _learnedSkills.GetValueOrDefault(skillId, 0);

    public (bool ok, int slot2, int skill2) EquipSkillSlot(SqliteDatabaseManager db, long unitUid, int slotId, int skillId)
    {
        if (slotId < 0 || slotId >= MaxSkillSlots)
            return (false, -1, 0);

        // If skill is already in another slot, swap cleanly (exact C++ SERV_SKILL_SLOT_CHANGE_PACKET_INTEGRATE behavior)
        int slot2 = -1;
        int skill2 = 0;
        if (skillId > 0)
        {
            for (int i = 0; i < MaxSkillSlots; i++)
            {
                if (i != slotId && _equippedSlots[i] == skillId)
                {
                    slot2 = i;
                    skill2 = _equippedSlots[slotId];
                    _equippedSlots[i] = skill2;
                    db.SetUnitSkillSlot(unitUid, i, skill2);
                    break;
                }
            }
        }

        _equippedSlots[slotId] = skillId;
        db.SetUnitSkillSlot(unitUid, slotId, skillId);
        return (true, slot2, skill2);
    }

    public void ResetSkillPage(SqliteDatabaseManager db, long unitUid, int unitLevel)
    {
        _learnedSkills.Clear();
        Array.Fill(_equippedSlots, 0);
        for (int i = 0; i < MaxSkillSlots; i++)
            db.SetUnitSkillSlot(unitUid, i, 0);
        AvailableSkillPoints = Math.Max(0, (unitLevel - 1) * 2 + 2);
    }

    public IReadOnlyList<int> GetEquippedSlots() => _equippedSlots;

    public void SetAvailableSkillPoints(int page, int sp)
    {
        ActiveSkillPage = Math.Max(0, page);
        AvailableSkillPoints = Math.Max(0, sp);
    }

    public bool LearnOrSetSkill(int skillId, int level)
    {
        if (skillId <= 0 || level <= 0) return false;
        _learnedSkills[skillId] = Math.Clamp(level, 1, 20);
        return true;
    }

    public Dictionary<int, int> GetLearnedSkillsSnapshot() => new(_learnedSkills);

    public Dictionary<int, int> LearnAllDefaultSkills(sbyte unitClass)
    {
        int baseSkill = Math.Max(1, (int)unitClass) * 100;
        for (int i = 1; i <= 6; i++)
            _learnedSkills[baseSkill + i] = 5;
        return new Dictionary<int, int>(_learnedSkills);
    }

    public List<KncWX2Server.Core.Packets.KUserSkillPageData> GetAllSkillPageDatas()
    {
        var list = new List<KncWX2Server.Core.Packets.KUserSkillPageData>(TheNumberOfSkillPagesAvailable);
        for (int p = 1; p <= TheNumberOfSkillPagesAvailable; p++)
        {
            int sp = p == ActiveSkillPage ? AvailableSkillPoints : _pageSkillPoints.GetValueOrDefault(p, AvailableSkillPoints);
            list.Add(BuildSkillPageData(sp, AvailableCashSkillPoints));
        }
        return list;
    }

    public List<short> GetUnsealedSkillIds()
        => _unsealedSkills.Select(id => (short)id).ToList();

    public Dictionary<sbyte, int> GetSkillNotePacketMap()
    {
        var map = new Dictionary<sbyte, int>();
        foreach (var kv in _skillNotePages)
        {
            if (kv.Key is >= sbyte.MinValue and <= sbyte.MaxValue)
                map[(sbyte)kv.Key] = kv.Value;
        }
        return map;
    }
}
