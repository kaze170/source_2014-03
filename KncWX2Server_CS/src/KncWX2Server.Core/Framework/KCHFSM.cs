namespace KncWX2Server.Core.Framework;

public sealed class FSMState
{
    private readonly Dictionary<int, int> _transitions = new();

    public int StateId { get; }

    public FSMState(int stateId)
    {
        StateId = stateId;
    }

    public void AddTransition(int input, int outputState) => _transitions[input] = outputState;

    public int GetOutput(int input)
        => _transitions.TryGetValue(input, out int nextState) ? nextState : throw new InvalidOperationException($"Invalid FSM transition from state {StateId} on input {input}.");

    public bool TryGetOutput(int input, out int outputState)
        => _transitions.TryGetValue(input, out outputState);
}

public class FSMClass
{
    private readonly Dictionary<int, FSMState> _states = new();

    public int CurrentState { get; protected set; }
    public int StateId => CurrentState;
    public int CurrentStateId => CurrentState;

    public FSMClass(int initialState)
    {
        CurrentState = initialState;
    }

    public void AddState(FSMState state) => _states[state.StateId] = state;

    public int StateTransition(int input)
    {
        if (!_states.TryGetValue(CurrentState, out var state))
            throw new InvalidOperationException($"Unknown FSM state {CurrentState}.");

        if (!state.TryGetOutput(input, out int next))
            throw new InvalidOperationException($"Invalid transition: {GetStateIDString(CurrentState)} + {GetInputIDString(input)}.");

        CurrentState = next;
        return next;
    }

    public bool TryStateTransition(int input, out int nextState)
    {
        nextState = CurrentState;
        if (!_states.TryGetValue(CurrentState, out var state) || !state.TryGetOutput(input, out int next))
            return false;

        CurrentState = next;
        nextState = next;
        return true;
    }

    public virtual string GetStateIDString(int stateId) => stateId.ToString();
    public virtual string GetInputIDString(int inputId) => inputId.ToString();
}

/// <summary>
/// Port of Common/CHFSM.{h,cpp} for ChannelUser connection states.
/// </summary>
public sealed class KCHFSM : FSMClass
{
    public const int STATE_ZERO_NO_USE = 0;
    public const int S_INIT = 1;
    public const int S_LOGINED = 2;
    public const int S_WAIT_QUEUE = 3;
    public const int S_EXIT = 4;
    public const int STATE_SENTINEL = 5;

    public const int I_TO_AUTHENTICATED = 0;
    public const int I_TO_WAIT_QUEUE = 1;
    public const int I_TO_EXIT = 2;

    public KCHFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_AUTHENTICATED, S_LOGINED);
        sInit.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sInit);

        var sLogined = new FSMState(S_LOGINED);
        sLogined.AddTransition(I_TO_WAIT_QUEUE, S_WAIT_QUEUE);
        sLogined.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sLogined);

        var sWaitQueue = new FSMState(S_WAIT_QUEUE);
        sWaitQueue.AddTransition(I_TO_AUTHENTICATED, S_LOGINED);
        sWaitQueue.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sWaitQueue);

        AddState(new FSMState(S_EXIT));
    }

    public override string GetStateIDString(int stateId) => stateId switch
    {
        STATE_ZERO_NO_USE => "KCHFSM::STATE_ZERO_NO_USE",
        S_INIT => "KCHFSM::S_INIT",
        S_LOGINED => "KCHFSM::S_LOGINED",
        S_WAIT_QUEUE => "KCHFSM::S_WAIT_QUEUE",
        S_EXIT => "KCHFSM::S_EXIT",
        STATE_SENTINEL => "KCHFSM::STATE_SENTINEL",
        _ => $"KCHFSM::UnknownState({stateId})",
    };

    public override string GetInputIDString(int inputId) => inputId switch
    {
        I_TO_AUTHENTICATED => "KCHFSM::I_TO_AUTHENTICATED",
        I_TO_WAIT_QUEUE => "KCHFSM::I_TO_WAIT_QUEUE",
        I_TO_EXIT => "KCHFSM::I_TO_EXIT",
        _ => $"KCHFSM::UnknownInput({inputId})",
    };
}
