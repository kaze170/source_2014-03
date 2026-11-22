namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/ActionDecider.{h,cpp} providing probabilistic FSM action selection and Lua AddAction binding.
/// </summary>
public sealed class KActionDecider
{
    public const int DefaultInvalidAction = -1;

    private sealed class KActionUnit
    {
        private readonly List<(int Action, float Prob)> _set = [];
        private float _accProb;

        public void AddActionProbability(int action, float prob)
        {
            if (prob <= 0f) return;
            _accProb += prob;
            if (_accProb > 1.0f) _accProb = 1.0f;
            _set.Add((action, _accProb));
        }

        public bool Decision(float roulette, out int action)
        {
            foreach (var (a, cumulative) in _set)
            {
                if (roulette <= cumulative)
                {
                    action = a;
                    return true;
                }
            }
            action = DefaultInvalidAction;
            return false;
        }
    }

    private readonly Dictionary<int, KActionUnit> _transitionTable = new();
    private readonly int _invalidAction;

    public KActionDecider(int invalidAction = DefaultInvalidAction)
    {
        _invalidAction = invalidAction;
    }

    public int StateCount => _transitionTable.Count;

    public void AddAction(int iState, int iAction, float fProb)
    {
        if (!_transitionTable.TryGetValue(iState, out var unit))
        {
            unit = new KActionUnit();
            _transitionTable[iState] = unit;
        }
        unit.AddActionProbability(iAction, fProb);
    }

    public Dictionary<int, int> MakeDecision()
    {
        var result = new Dictionary<int, int>();
        foreach (int state in _transitionTable.Keys)
        {
            int action = MakeDecision(state);
            if (action != _invalidAction)
                result[state] = action;
        }
        return result;
    }

    public int MakeDecision(int iState)
        => MakeDecision(iState, Random.Shared.NextSingle());

    public int MakeDecision(int iState, float fRoulette)
    {
        if (_transitionTable.TryGetValue(iState, out var unit) && unit.Decision(fRoulette, out int action))
            return action;
        return _invalidAction;
    }
}
