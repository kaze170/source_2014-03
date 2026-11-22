using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserCashInventory.{h,cpp}:
/// tracks real-time cash resurrection stones, package pickup slots, gift metadata,
/// and cash item socket option mappings.
/// </summary>
public sealed class KUserCashInventory
{
    public sealed class CashProductRule
    {
        public uint ProductNo { get; init; }
        public int Price { get; init; }
        public int MaxPurchasePerAccount { get; init; }
        public byte MinLevel { get; init; } = 1;
        public sbyte RequiredUnitType { get; init; } = -1; // -1 = any
        public bool IsGiftable { get; init; } = true;
    }

    private readonly Dictionary<uint, List<KInventoryItemInfo>> _packagePickups = new();
    private readonly Dictionary<int, int> _cashItemSockets = new();
    private readonly Dictionary<uint, CashProductRule> _productRules = new();
    private readonly Dictionary<uint, int> _purchasedCounts = new();

    public bool IsCashResurrectionStone { get; set; }
    public long CashResurrectionStoneUnitUid { get; set; }
    public string GiftReceiverUserId { get; set; } = string.Empty;
    public string GiftMessage { get; set; } = string.Empty;
    public bool IsInCashShop { get; set; }
    public int CashSkillPoint { get; set; }
    public string CashSkillPointEndDate { get; set; } = string.Empty;
    public long WarpVipEndDate { get; set; }
    public KGlobalCashInfo GlobalCashInfo { get; } = new();

    // Bingo Event state (exact C++ Temp.{h,cpp} KGSBingoEvent 5x5 = 25 cells, 12 lines, BSV_NUM_MAX = 100)
    public const int BingoBoardLineSize = 5;
    public const int BingoBoardSlotNum = BingoBoardLineSize * BingoBoardLineSize; // 25
    public const int BingoPresentNum = 12;
    public const int BingoNumMax = 100;
    public const int BingoMixRestrictOpenCount = 10;

    public int BingoLotteryChance { get; set; } = 3;
    public int BingoLotteryRecord { get; set; }
    public int BingoMixChance { get; set; } = 1;
    public int BingoResetChance { get; set; } = 1;
    public byte[] BingoBoard { get; private set; }
    public List<(int PresentId, bool Acquired)> BingoPresents { get; } = [];
    public HashSet<(uint OrderNo, uint ProductNo)> CheckedPresentOrders { get; } = [];
    private bool _bingoInitializedFromServer;

    public KUserCashInventory()
    {
        BingoBoard = new byte[BingoBoardSlotNum];
        InitDefaultBingoBoard();
    }

    private void InitDefaultBingoBoard()
    {
        for (int i = 0; i < BingoBoardSlotNum; i++)
            BingoBoard[i] = (byte)i;
        OpenCornerCells();
        BingoPresents.Clear();
        for (int i = 0; i < BingoPresentNum; i++)
            BingoPresents.Add((130001 + i, false));
    }

    private void OpenCornerCells()
    {
        if (BingoBoard.Length < BingoBoardSlotNum) return;
        BingoBoard[0] += BingoNumMax;
        BingoBoard[BingoBoardLineSize - 1] += BingoNumMax;
        BingoBoard[BingoBoardSlotNum - BingoBoardLineSize] += BingoNumMax;
        BingoBoard[BingoBoardSlotNum - 1] += BingoNumMax;
    }

    public void EnsureBingoInitialized(
        IReadOnlySet<int>? banSpots = null,
        IReadOnlyDictionary<int, (uint ProductNo, bool IsBanPresent)>? presentInfos = null)
    {
        if (_bingoInitializedFromServer) return;
        if (presentInfos != null && presentInfos.Count >= BingoPresentNum)
            BingoReset(isInit: true, banSpots, presentInfos);
        _bingoInitializedFromServer = true;
    }

    public void Clear()
    {
        IsCashResurrectionStone = false;
        CashResurrectionStoneUnitUid = 0;
        GiftReceiverUserId = string.Empty;
        GiftMessage = string.Empty;
        IsInCashShop = false;
        CashSkillPoint = 0;
        CashSkillPointEndDate = string.Empty;
        WarpVipEndDate = 0;
        Array.Clear(GlobalCashInfo.Cash);
        _packagePickups.Clear();
        _cashItemSockets.Clear();
        CheckedPresentOrders.Clear();
    }

    private void InsertLotteryNum(byte num, List<byte> acquiredPresent)
    {
        int pos = -1;
        for (int i = 0; i < BingoBoard.Length; i++)
        {
            if (BingoBoard[i] == num)
            {
                pos = i;
                break;
            }
        }
        if (pos < 0) return;

        BingoBoard[pos] += BingoNumMax;

        // 1. Row check (lines 0..4)
        int rowStart = (pos / BingoBoardLineSize) * BingoBoardLineSize;
        bool rowComplete = true;
        for (int i = 0; i < BingoBoardLineSize; i++)
        {
            if (BingoBoard[rowStart + i] < BingoNumMax) { rowComplete = false; break; }
        }
        if (rowComplete)
        {
            byte lineIdx = (byte)(rowStart / BingoBoardLineSize);
            if (lineIdx < BingoPresents.Count && !BingoPresents[lineIdx].Acquired)
            {
                BingoPresents[lineIdx] = (BingoPresents[lineIdx].PresentId, true);
                acquiredPresent.Add(lineIdx);
            }
        }

        // 2. Column check (lines 5..9)
        int col = pos % BingoBoardLineSize;
        bool colComplete = true;
        for (int i = 0; i < BingoBoardSlotNum; i += BingoBoardLineSize)
        {
            if (BingoBoard[col + i] < BingoNumMax) { colComplete = false; break; }
        }
        if (colComplete)
        {
            byte lineIdx = (byte)(col + BingoBoardLineSize);
            if (lineIdx < BingoPresents.Count && !BingoPresents[lineIdx].Acquired)
            {
                BingoPresents[lineIdx] = (BingoPresents[lineIdx].PresentId, true);
                acquiredPresent.Add(lineIdx);
            }
        }

        // 3. Main diagonal '\' (line 11)
        int diag1Step = BingoBoardLineSize + 1;
        if (pos % diag1Step == 0 && pos / diag1Step < BingoBoardLineSize)
        {
            bool d1 = true;
            for (int i = 0; i < BingoBoardLineSize; i++)
            {
                if (BingoBoard[i * diag1Step] < BingoNumMax) { d1 = false; break; }
            }
            if (d1)
            {
                byte lineIdx = BingoPresentNum - 1;
                if (lineIdx < BingoPresents.Count && !BingoPresents[lineIdx].Acquired)
                {
                    BingoPresents[lineIdx] = (BingoPresents[lineIdx].PresentId, true);
                    acquiredPresent.Add(lineIdx);
                }
            }
        }

        // 4. Anti-diagonal '/' (line 10)
        int diag2Step = BingoBoardLineSize - 1;
        if (pos % diag2Step == 0 && pos / diag2Step <= BingoBoardLineSize)
        {
            bool d2 = true;
            for (int i = 1; i <= BingoBoardLineSize; i++)
            {
                if (BingoBoard[i * diag2Step] < BingoNumMax) { d2 = false; break; }
            }
            if (d2)
            {
                byte lineIdx = BingoPresentNum - 2;
                if (lineIdx < BingoPresents.Count && !BingoPresents[lineIdx].Acquired)
                {
                    BingoPresents[lineIdx] = (BingoPresents[lineIdx].PresentId, true);
                    acquiredPresent.Add(lineIdx);
                }
            }
        }
    }

    private List<byte> MixBoardInternal(bool onlyMove)
    {
        var openNums = new List<byte>();
        if (onlyMove)
        {
            var candidates = new List<byte>(BingoBoardSlotNum);
            foreach (byte val in BingoBoard)
            {
                byte raw = val;
                if (raw >= BingoNumMax)
                {
                    raw -= BingoNumMax;
                    openNums.Add(raw);
                }
                candidates.Add(raw);
            }
            for (int i = 0; i < BingoBoardSlotNum && candidates.Count > 0; i++)
            {
                int idx = Random.Shared.Next(candidates.Count);
                BingoBoard[i] = candidates[idx];
                candidates.RemoveAt(idx);
            }
        }
        else
        {
            var candidates = new List<byte>(BingoNumMax);
            for (byte n = 0; n < BingoNumMax; n++)
                candidates.Add(n);
            for (int i = 0; i < BingoBoardSlotNum; i++)
            {
                int idx = Random.Shared.Next(candidates.Count);
                BingoBoard[i] = candidates[idx];
                candidates.RemoveAt(idx);
            }
        }
        return openNums;
    }

    private bool MixPresentsInternal(
        IReadOnlySet<int>? banSpots,
        IReadOnlyDictionary<int, (uint ProductNo, bool IsBanPresent)>? presentInfos)
    {
        var normal = new List<int>();
        var ban = new List<int>();
        if (presentInfos != null && presentInfos.Count >= BingoPresentNum)
        {
            foreach (var kv in presentInfos)
            {
                if (kv.Value.IsBanPresent) ban.Add(kv.Key);
                else normal.Add(kv.Key);
            }
        }
        else
        {
            for (int i = 0; i < BingoPresentNum; i++)
                normal.Add(130001 + i);
        }

        var next = new (int PresentId, bool Acquired)[BingoPresentNum];
        var spots = Enumerable.Range(0, BingoPresentNum).ToList();
        while (spots.Count > 0)
        {
            int pick = Random.Shared.Next(spots.Count);
            int spot = spots[pick];
            spots.RemoveAt(pick);

            bool isBanSpot = banSpots != null && banSpots.Contains(spot);
            if (isBanSpot)
            {
                if (normal.Count == 0) return false;
                int item = normal[^1];
                normal.RemoveAt(normal.Count - 1);
                next[spot] = (item, false);
            }
            else if (ban.Count > 0)
            {
                int item = ban[^1];
                ban.RemoveAt(ban.Count - 1);
                next[spot] = (item, false);
            }
            else if (normal.Count > 0)
            {
                int item = normal[^1];
                normal.RemoveAt(normal.Count - 1);
                next[spot] = (item, false);
            }
            else
            {
                return false;
            }
        }

        BingoPresents.Clear();
        BingoPresents.AddRange(next);
        return true;
    }

    public (int ok, byte num, List<byte> acquired) BingoLottery()
    {
        if (BingoLotteryChance <= 0)
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_01, 0, []);
        var unopened = new List<byte>();
        foreach (byte val in BingoBoard)
        {
            if (val < BingoNumMax) unopened.Add(val);
        }
        if (unopened.Count == 0)
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_02, 0, []);

        BingoLotteryChance--;
        BingoLotteryRecord++;
        byte num = unopened[Random.Shared.Next(unopened.Count)];
        var acq = new List<byte>();
        InsertLotteryNum(num, acq);
        return (KncWX2Server.Core.Region.NetError.NET_OK, num, acq);
    }

    public (int ok, List<byte> board, List<byte> acquired) BingoMix()
    {
        if (BingoMixChance <= 0)
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_03, [.. BingoBoard], []);
        if (BingoPresents.Any(p => p.Acquired))
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_03, [.. BingoBoard], []);
        if (BingoBoard.Count(b => b >= BingoNumMax) >= BingoMixRestrictOpenCount)
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_03, [.. BingoBoard], []);

        BingoMixChance--;
        var openNums = MixBoardInternal(onlyMove: true);
        var acq = new List<byte>();
        foreach (byte openNum in openNums)
            InsertLotteryNum(openNum, acq);
        return (KncWX2Server.Core.Region.NetError.NET_OK, [.. BingoBoard], acq);
    }

    public (int ok, List<byte> board, List<int> presentIds) BingoReset(
        bool isInit,
        IReadOnlySet<int>? banSpots = null,
        IReadOnlyDictionary<int, (uint ProductNo, bool IsBanPresent)>? presentInfos = null)
    {
        if (!isInit && BingoResetChance <= 0)
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_03, [.. BingoBoard], BingoPresents.Select(p => p.PresentId).ToList());
        if (!MixPresentsInternal(banSpots, presentInfos))
            return (KncWX2Server.Core.Region.NetError.ERR_BINGO_03, [.. BingoBoard], BingoPresents.Select(p => p.PresentId).ToList());

        MixBoardInternal(onlyMove: false);
        OpenCornerCells();
        if (!isInit) BingoResetChance--;
        _bingoInitializedFromServer = true;
        return (KncWX2Server.Core.Region.NetError.NET_OK, [.. BingoBoard], BingoPresents.Select(p => p.PresentId).ToList());
    }

    public void RegisterProductRule(CashProductRule rule) => _productRules[rule.ProductNo] = rule;

    public (bool ok, int netError) ValidatePurchase(uint productNo, int quantity, byte unitLevel, sbyte unitType, bool isGift)
    {
        if (quantity <= 0) return (false, KncWX2Server.Core.Region.NetError.ERR_BUY_CASH_ITEM_01);
        if (!_productRules.TryGetValue(productNo, out var rule))
            return (true, KncWX2Server.Core.Region.NetError.NET_OK);

        if (isGift && !rule.IsGiftable)
            return (false, KncWX2Server.Core.Region.NetError.ERR_BUY_CASH_ITEM_01);
        if (unitLevel < rule.MinLevel)
            return (false, KncWX2Server.Core.Region.NetError.ERR_BUY_CASH_ITEM_01);
        if (rule.RequiredUnitType >= 0 && rule.RequiredUnitType != unitType)
            return (false, KncWX2Server.Core.Region.NetError.ERR_BUY_CASH_ITEM_01);
        if (rule.MaxPurchasePerAccount > 0)
        {
            int cur = _purchasedCounts.GetValueOrDefault(productNo);
            if (cur + quantity > rule.MaxPurchasePerAccount)
                return (false, KncWX2Server.Core.Region.NetError.ERR_BUY_CASH_ITEM_01);
        }
        return (true, KncWX2Server.Core.Region.NetError.NET_OK);
    }

    public void RecordPurchase(uint productNo, int quantity)
        => _purchasedCounts[productNo] = _purchasedCounts.GetValueOrDefault(productNo) + Math.Max(0, quantity);

    public void AddPickUpPackageInfo(uint productNo, IEnumerable<KInventoryItemInfo> items)
        => _packagePickups[productNo] = items.ToList();

    public bool TryPickUpPackage(uint productNo, out List<KInventoryItemInfo> items)
    {
        if (_packagePickups.TryGetValue(productNo, out var found))
        {
            items = found;
            _packagePickups.Remove(productNo);
            return true;
        }
        items = [];
        return false;
    }

    public void SetSocketForCash(IReadOnlyDictionary<int, int> socketMap)
    {
        foreach (var kv in socketMap)
            _cashItemSockets[kv.Key] = kv.Value;
    }

    public bool TryGetSocketOptionForCash(int itemId, out int socketOptionId)
        => _cashItemSockets.TryGetValue(itemId, out socketOptionId);
}
