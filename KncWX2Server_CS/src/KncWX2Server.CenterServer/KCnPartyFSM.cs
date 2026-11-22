using KncWX2Server.Core.Framework;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/CnPartyFSM.{h,cpp}.
/// </summary>
public sealed class KCnPartyFSM : FSMClass
{
    public const int S_INIT = 0;
    public const int S_CLOSE = 1;
    public const int S_MAKING_PARTY = 2;
    public const int S_WAIT = 3;
    public const int S_CHECK_PLAY_CONDITION = 4;
    public const int S_CHECK_AUTO_PARTY = 5;
    public const int S_ACCEPT_FOR_PLAY = 6;
    public const int S_PREPARE_FOR_PLAY = 7;

    public const int I_TO_INIT = 1;
    public const int I_TO_CLOSE = 2;
    public const int I_TO_MAKING_PARTY = 3;
    public const int I_TO_WAIT = 4;
    public const int I_TO_CHECK_PLAY_CONDITION = 5;
    public const int I_TO_CHECK_AUTO_PARTY = 6;
    public const int I_TO_ACCEPT_FOR_PLAY = 7;
    public const int I_TO_PREPARE_FOR_PLAY = 8;

    public KCnPartyFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_CLOSE, S_CLOSE);
        sInit.AddTransition(I_TO_MAKING_PARTY, S_MAKING_PARTY);
        sInit.AddTransition(I_TO_WAIT, S_WAIT);
        AddState(sInit);

        var sClose = new FSMState(S_CLOSE);
        sClose.AddTransition(I_TO_INIT, S_INIT);
        AddState(sClose);

        var sMaking = new FSMState(S_MAKING_PARTY);
        sMaking.AddTransition(I_TO_CLOSE, S_CLOSE);
        sMaking.AddTransition(I_TO_WAIT, S_WAIT);
        AddState(sMaking);

        var sWait = new FSMState(S_WAIT);
        sWait.AddTransition(I_TO_CLOSE, S_CLOSE);
        sWait.AddTransition(I_TO_CHECK_PLAY_CONDITION, S_CHECK_PLAY_CONDITION);
        sWait.AddTransition(I_TO_CHECK_AUTO_PARTY, S_CHECK_AUTO_PARTY);
        sWait.AddTransition(I_TO_ACCEPT_FOR_PLAY, S_ACCEPT_FOR_PLAY);
        sWait.AddTransition(I_TO_PREPARE_FOR_PLAY, S_PREPARE_FOR_PLAY);
        AddState(sWait);

        var sCheckPlay = new FSMState(S_CHECK_PLAY_CONDITION);
        sCheckPlay.AddTransition(I_TO_CLOSE, S_CLOSE);
        sCheckPlay.AddTransition(I_TO_WAIT, S_WAIT);
        sCheckPlay.AddTransition(I_TO_CHECK_AUTO_PARTY, S_CHECK_AUTO_PARTY);
        sCheckPlay.AddTransition(I_TO_PREPARE_FOR_PLAY, S_PREPARE_FOR_PLAY);
        AddState(sCheckPlay);

        var sCheckAuto = new FSMState(S_CHECK_AUTO_PARTY);
        sCheckAuto.AddTransition(I_TO_CLOSE, S_CLOSE);
        sCheckAuto.AddTransition(I_TO_WAIT, S_WAIT);
        sCheckAuto.AddTransition(I_TO_ACCEPT_FOR_PLAY, S_ACCEPT_FOR_PLAY);
        AddState(sCheckAuto);

        var sAccept = new FSMState(S_ACCEPT_FOR_PLAY);
        sAccept.AddTransition(I_TO_CLOSE, S_CLOSE);
        sAccept.AddTransition(I_TO_WAIT, S_WAIT);
        sAccept.AddTransition(I_TO_PREPARE_FOR_PLAY, S_PREPARE_FOR_PLAY);
        AddState(sAccept);

        var sPrepare = new FSMState(S_PREPARE_FOR_PLAY);
        sPrepare.AddTransition(I_TO_CLOSE, S_CLOSE);
        sPrepare.AddTransition(I_TO_WAIT, S_WAIT);
        AddState(sPrepare);
    }
}
