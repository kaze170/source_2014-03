namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLDungeonStage &amp; XSLDungeonSubStage (SERV_STAGE_CLEAR_IN_SERVER):
/// models dungeon stage/sub-stage graphs, weighted next-stage transitions, and secret/elite stage selection.
/// </summary>
public sealed class KDungeonSubStageManager
{
    public enum ClearType : int
    {
        CtSubStage = 0,
        CtStage = 1,
        CtStageForce = 2,
        CtStageSecret = 3,
        CtGame = 4,
        CtFailure = 5,
    }

    public enum GoType : int
    {
        GtNone = 0,
        GtRight = 1,
        GtLeft = 2,
        GtUp = 3,
        GtDown = 4,
    }

    public enum SecretStagePad : int
    {
        SspNone = 0,
        SspSecret = 1,
        SspElite = 2,
        SspNormal = 3,
    }

    public sealed record NextStageData(
        ClearType ClearType,
        int StageIndex,
        int SubStageIndex,
        int Rate = 100,
        int EndingSpeechTableIndex = 0);

    public sealed class SubStageTemplate
    {
        public int SubStageIndex { get; init; }
        public GoType GoType { get; init; } = GoType.GtRight;
        public SecretStagePad SecretPad { get; init; } = SecretStagePad.SspNone;
        public List<NextStageData> NextStages { get; } = new();
    }

    public sealed class StageTemplate
    {
        public int StageIndex { get; init; }
        public Dictionary<int, SubStageTemplate> SubStages { get; } = new();
    }

    public sealed class DungeonStageGraph
    {
        public int DungeonId { get; init; }
        public Dictionary<int, StageTemplate> Stages { get; } = new();
    }

    private readonly Dictionary<int, DungeonStageGraph> _dungeons = new();
    private readonly Random _rng = new();

    public void RegisterDungeonGraph(DungeonStageGraph graph)
        => _dungeons[graph.DungeonId] = graph;

    public bool TryGetGraph(int dungeonId, out DungeonStageGraph? graph)
        => _dungeons.TryGetValue(dungeonId, out graph);

    public NextStageData ResolveNextStage(
        int dungeonId,
        int currentStageIndex,
        int currentSubStageIndex,
        SecretStagePad requestedPad = SecretStagePad.SspNone,
        double? fixedUnitRoll = null)
    {
        if (!_dungeons.TryGetValue(dungeonId, out var graph) ||
            !graph.Stages.TryGetValue(currentStageIndex, out var stage) ||
            !stage.SubStages.TryGetValue(currentSubStageIndex, out var subStage) ||
            subStage.NextStages.Count == 0)
        {
            return new NextStageData(ClearType.CtGame, -1, -1, 100);
        }

        if (requestedPad == SecretStagePad.SspSecret)
        {
            var secret = subStage.NextStages.FirstOrDefault(n => n.ClearType == ClearType.CtStageSecret);
            if (secret != null)
                return secret;
        }

        int totalRate = subStage.NextStages.Sum(n => Math.Max(0, n.Rate));
        if (totalRate <= 0)
            return subStage.NextStages[0];

        double roll = (fixedUnitRoll ?? _rng.NextDouble()) * totalRate;
        double acc = 0;
        foreach (var cand in subStage.NextStages)
        {
            acc += Math.Max(0, cand.Rate);
            if (roll <= acc)
                return cand;
        }
        return subStage.NextStages[^1];
    }

    public static bool EvaluateSecretStageEntry(
        int averagePartyLevel,
        IReadOnlyList<(int MinLv, int MaxLv, float Rate)> conditions,
        double? fixedUnitRoll = null)
    {
        if (conditions.Count == 0) return false;
        foreach (var (minLv, maxLv, rate) in conditions)
        {
            if (averagePartyLevel >= minLv && averagePartyLevel <= maxLv)
            {
                float clamped = Math.Clamp(rate, 0f, 100f);
                double roll = (fixedUnitRoll ?? Random.Shared.NextDouble()) * 100.0;
                return roll < clamped;
            }
        }
        return false;
    }
}
