namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/Lottery.{h,cpp} (KLottery):
/// deterministic-or-PRNG 0..100% roulette wheel with integer-percent / float-percent cases and Param1/Param2.
/// </summary>
public sealed class KLottery
{
    public const int CaseBlank = -1;
    public const int ParamBlank = -2;

    public readonly record struct CaseUnit(double Probability, int Param1, int Param2);

    private readonly SortedDictionary<int, CaseUnit> _cases = new();
    private readonly Random _rng;

    public double TotalProbability { get; private set; }
    public int CaseCount => _cases.Count;
    public int GetCaseNum() => _cases.Count;

    public KLottery(int? seed = null)
    {
        _rng = seed.HasValue ? new Random(seed.Value) : Random.Shared;
    }

    public void Clear()
    {
        _cases.Clear();
        TotalProbability = 0.0;
    }

    public bool IsExistCase(int caseId) => _cases.ContainsKey(caseId);

    public IReadOnlyList<int> GetCaseList() => _cases.Keys.ToList();

    public int GetFirstCase() => _cases.Count > 0 ? _cases.Keys.First() : CaseBlank;

    public KLottery Clone()
    {
        var copy = new KLottery();
        foreach (var kv in _cases)
        {
            copy._cases[kv.Key] = kv.Value;
        }
        copy.TotalProbability = TotalProbability;
        return copy;
    }

    public bool DelProbability(int caseId)
    {
        if (!_cases.Remove(caseId, out var removed))
            return false;
        TotalProbability = Math.Max(0.0, TotalProbability - removed.Probability);
        return true;
    }

    public bool AddCase(int caseId, double probability, int param1 = ParamBlank, int param2 = ParamBlank)
    {
        if (probability <= 0.0) return false;
        if (TotalProbability + probability > 100.0001)
            return false;

        if (_cases.TryGetValue(caseId, out var existing))
        {
            _cases[caseId] = new CaseUnit(existing.Probability + probability, param1, param2);
            TotalProbability += probability;
        }
        else
        {
            _cases[caseId] = new CaseUnit(probability, param1, param2);
            TotalProbability += probability;
        }
        return true;
    }

    public bool AddCaseIntegerCast(int caseId, double probability, int param1 = ParamBlank, int param2 = ParamBlank)
    {
        if ((int)(TotalProbability + probability) > 100)
            return false;
        return AddCase(caseId, probability, param1, param2);
    }

    public int Decision() => Decision(out _);

    public int Decision(double? fixedUnitRoll)
    {
        if (!fixedUnitRoll.HasValue)
            return Decision();
        double roulette = Math.Clamp(fixedUnitRoll.Value, 0.0, 1.0) * 100.0;
        double accumulate = 0.0;
        foreach (var kv in _cases)
        {
            accumulate += kv.Value.Probability;
            if (roulette <= accumulate)
                return kv.Key;
        }
        return CaseBlank;
    }

    public int DecisionWithRate(double rateMultiplier)
    {
        if (rateMultiplier <= 0.0)
            return CaseBlank;
        if (Math.Abs(rateMultiplier - 1.0) < 1e-6)
            return Decision();
        var copy = Clone();
        copy.AddMultiProbRate(rateMultiplier);
        return copy.Decision();
    }

    public int Decision(out double checkRoulette)
    {
        checkRoulette = _rng.NextDouble() * 100.0;
        double accumulate = 0.0;
        foreach (var kv in _cases)
        {
            accumulate += kv.Value.Probability;
            if (checkRoulette <= accumulate)
                return kv.Key;
        }
        return CaseBlank;
    }

    public int GetParam1(int caseId) => _cases.TryGetValue(caseId, out var u) ? u.Param1 : ParamBlank;
    public int GetParam2(int caseId) => _cases.TryGetValue(caseId, out var u) ? u.Param2 : ParamBlank;

    public bool AddMultiProbRate(double rate)
    {
        if (rate <= 0.0 || _cases.Count == 0 || TotalProbability <= 0.0)
            return false;
        double effectiveRate = (TotalProbability * rate > 100.0) ? (100.0 / TotalProbability) : rate;
        TotalProbability = 0.0;
        foreach (int key in _cases.Keys.ToList())
        {
            var cur = _cases[key];
            double nextProb = cur.Probability * effectiveRate;
            _cases[key] = cur with { Probability = nextProb };
            TotalProbability += nextProb;
        }
        return true;
    }

    public bool MakeHundredProbability()
    {
        if (_cases.Count == 0 || TotalProbability <= 0.0) return false;
        double scale = 100.0 / TotalProbability;
        foreach (int key in _cases.Keys.ToList())
        {
            var cur = _cases[key];
            _cases[key] = cur with { Probability = cur.Probability * scale };
        }
        TotalProbability = 100.0;
        return true;
    }
}
