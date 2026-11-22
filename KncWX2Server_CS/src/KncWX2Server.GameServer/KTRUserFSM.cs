using KncWX2Server.Core.Framework;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/TRUserFSM.{h,cpp}.
/// </summary>
public sealed class KTRUserFSM : FSMClass
{
    public const int STATE_EXIT = 0;
    public const int STATE_INIT = 1;
    public const int STATE_CONNECTED = 2;
    public const int STATE_REGISTERING = 3;
    public const int STATE_REGISTERED = 4;

    public const int INPUT_CONNECTED = 1;
    public const int INPUT_REG_UID_REQ = 2;
    public const int INPUT_REG_UID_ACK = 3;
    public const int INPUT_EXIT = 99;

    public KTRUserFSM() : base(STATE_INIT)
    {
        var sInit = new FSMState(STATE_INIT);
        sInit.AddTransition(INPUT_CONNECTED, STATE_CONNECTED);
        sInit.AddTransition(INPUT_REG_UID_REQ, STATE_REGISTERING);
        sInit.AddTransition(INPUT_REG_UID_ACK, STATE_REGISTERED);
        sInit.AddTransition(INPUT_EXIT, STATE_EXIT);
        AddState(sInit);

        var sConn = new FSMState(STATE_CONNECTED);
        sConn.AddTransition(INPUT_REG_UID_REQ, STATE_REGISTERING);
        sConn.AddTransition(INPUT_REG_UID_ACK, STATE_REGISTERED);
        sConn.AddTransition(INPUT_EXIT, STATE_EXIT);
        AddState(sConn);

        var sReging = new FSMState(STATE_REGISTERING);
        sReging.AddTransition(INPUT_REG_UID_ACK, STATE_REGISTERED);
        sReging.AddTransition(INPUT_EXIT, STATE_EXIT);
        AddState(sReging);

        var sReged = new FSMState(STATE_REGISTERED);
        sReged.AddTransition(INPUT_EXIT, STATE_EXIT);
        AddState(sReged);

        AddState(new FSMState(STATE_EXIT));
    }
}
