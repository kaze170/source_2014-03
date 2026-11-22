namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserStatistics.{h,cpp}:
/// records per-unit PvP, Dungeon, TrainingCenter, Etc, and ED statistics tables
/// (normal users only), tracks PlayTime categories, UsedSpirit, and account/char counts.
/// </summary>
public sealed class KUserStatistics
{
    public enum UsTable
    {
        PvpModeCount = 0,
        PvpPlayTime = 1,
        Dungeon = 2,
        TrainingCenter = 3,
        EtcData = 4,
        EdData = 5,
    }

    public enum PlayTimeType
    {
        TotalPlayTime = 0,
        PvpPlayTime = 1,
        SpiritPlayTime = 2,
        DungeonPlayTime = 3,
        FieldPlayTime = 4,
    }

    public enum AccountGameCountType
    {
        BuyCashItem = 0,
    }

    private readonly Dictionary<(int tableId, int subTableId), List<int>> _pvp = new();
    private readonly Dictionary<(int tableId, int subTableId), List<int>> _dungeon = new();
    private readonly Dictionary<(int tableId, int subTableId), List<int>> _training = new();
    private readonly Dictionary<(int tableId, int subTableId), List<int>> _etc = new();
    private readonly Dictionary<(int tableId, int subTableId), List<int>> _ed = new();
    private readonly int[] _playTime = new int[5];
    private readonly Dictionary<int, int> _accountCounts = new();

    public bool IsNormalUser { get; private set; } = true;
    public int UsedSpirit { get; private set; }

    public void SetIsNormalUser(sbyte authLevel)
        => IsNormalUser = authLevel == 0 || authLevel == 1; // UAL_NORMAL / UAL_BLOCKED_1

    public void Clear()
    {
        _pvp.Clear();
        _dungeon.Clear();
        _training.Clear();
        _etc.Clear();
        _ed.Clear();
        Array.Clear(_playTime);
        UsedSpirit = 0;
    }

    public void IncreaseCount(UsTable table, int subTableId, int column, int count)
    {
        if (!IsNormalUser || column < 0) return;

        var target = table switch
        {
            UsTable.PvpModeCount or UsTable.PvpPlayTime => _pvp,
            UsTable.Dungeon => _dungeon,
            UsTable.TrainingCenter => _training,
            UsTable.EtcData => _etc,
            UsTable.EdData => _ed,
            _ => null,
        };
        if (target == null) return;

        var key = ((int)table, subTableId);
        if (!target.TryGetValue(key, out var cols))
        {
            cols = [];
            target[key] = cols;
        }
        while (cols.Count <= column) cols.Add(0);
        cols[column] += count;
    }

    public int GetCount(UsTable table, int subTableId, int column)
    {
        var target = table switch
        {
            UsTable.PvpModeCount or UsTable.PvpPlayTime => _pvp,
            UsTable.Dungeon => _dungeon,
            UsTable.TrainingCenter => _training,
            UsTable.EtcData => _etc,
            UsTable.EdData => _ed,
            _ => null,
        };
        if (target == null || column < 0) return 0;
        return target.TryGetValue(((int)table, subTableId), out var cols) && column < cols.Count
            ? cols[column]
            : 0;
    }

    public void AddPlayTime(PlayTimeType type, int seconds)
    {
        if (!IsNormalUser || seconds <= 0) return;
        _playTime[(int)type] += seconds;
    }

    public int GetPlayTime(PlayTimeType type) => _playTime[(int)type];

    public void AddUsedSpirit(int spirit)
    {
        if (!IsNormalUser || spirit <= 0) return;
        UsedSpirit += spirit;
    }

    public void IncreaseAccountCount(AccountGameCountType type, int delta = 1)
        => _accountCounts[(int)type] = _accountCounts.GetValueOrDefault((int)type) + delta;

    public int GetAccountCount(AccountGameCountType type)
        => _accountCounts.GetValueOrDefault((int)type);

    public bool IsFirstBuy => GetAccountCount(AccountGameCountType.BuyCashItem) == 1;
}
