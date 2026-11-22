using KncWX2Server.Core.Framework;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/MatchFSM.{h,cpp}.
/// </summary>
public sealed class KMatchFSM : FSMClass
{
    public const int S_INIT = 0;
    public const int S_CLOSE = 1;
    public const int S_MATCH_MAKING = 2;
    public const int S_CHECK_ACCEPT = 3;
    public const int S_PREPARE_FOR_PLAY = 4;
    public const int S_PLAY = 5;

    public const int I_TO_INIT = 0;
    public const int I_TO_CLOSE = 1;
    public const int I_TO_MATCH_MAKING = 2;
    public const int I_TO_CHECK_ACCEPT = 3;
    public const int I_TO_PREPARE_FOR_PLAY = 4;
    public const int I_TO_PLAY = 5;

    public KMatchFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_MATCH_MAKING, S_MATCH_MAKING);
        sInit.AddTransition(I_TO_CHECK_ACCEPT, S_CHECK_ACCEPT);
        sInit.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sInit);

        var sMaking = new FSMState(S_MATCH_MAKING);
        sMaking.AddTransition(I_TO_CHECK_ACCEPT, S_CHECK_ACCEPT);
        sMaking.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sMaking);

        var sAccept = new FSMState(S_CHECK_ACCEPT);
        sAccept.AddTransition(I_TO_MATCH_MAKING, S_MATCH_MAKING);
        sAccept.AddTransition(I_TO_PREPARE_FOR_PLAY, S_PREPARE_FOR_PLAY);
        sAccept.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sAccept);

        var sPrepare = new FSMState(S_PREPARE_FOR_PLAY);
        sPrepare.AddTransition(I_TO_PLAY, S_PLAY);
        sPrepare.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sPrepare);

        var sPlay = new FSMState(S_PLAY);
        sPlay.AddTransition(I_TO_MATCH_MAKING, S_MATCH_MAKING);
        sPlay.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sPlay);

        var sClose = new FSMState(S_CLOSE);
        sClose.AddTransition(I_TO_INIT, S_INIT);
        AddState(sClose);
    }
}

/// <summary>
/// Faithful C# port of GlobalServer/AutoPartyFSM.{h,cpp}.
/// </summary>
public sealed class KAutoPartyFSM : FSMClass
{
    public const int S_INIT = 0;
    public const int S_CLOSE = 1;
    public const int S_AUTO_PARTY_MAKING = 2;
    public const int S_CHECK_ACCEPT = 3;
    public const int S_REGROUP_PARTY = 4;
    public const int S_PREPARE_FOR_PLAY = 4;
    public const int S_PLAY = 5;

    public const int I_TO_INIT = 0;
    public const int I_TO_CLOSE = 1;
    public const int I_TO_AUTO_PARTY_MAKING = 2;
    public const int I_TO_CHECK_ACCEPT = 3;
    public const int I_TO_REGROUP_PARTY = 4;
    public const int I_TO_PREPARE_FOR_PLAY = 4;
    public const int I_TO_PLAY = 5;

    public KAutoPartyFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_AUTO_PARTY_MAKING, S_AUTO_PARTY_MAKING);
        sInit.AddTransition(I_TO_CHECK_ACCEPT, S_CHECK_ACCEPT);
        sInit.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sInit);

        var sMaking = new FSMState(S_AUTO_PARTY_MAKING);
        sMaking.AddTransition(I_TO_CHECK_ACCEPT, S_CHECK_ACCEPT);
        sMaking.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sMaking);

        var sAccept = new FSMState(S_CHECK_ACCEPT);
        sAccept.AddTransition(I_TO_REGROUP_PARTY, S_REGROUP_PARTY);
        sAccept.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sAccept);

        var sRegroup = new FSMState(S_REGROUP_PARTY);
        sRegroup.AddTransition(I_TO_PLAY, S_PLAY);
        sRegroup.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sRegroup);

        var sPlay = new FSMState(S_PLAY);
        sPlay.AddTransition(I_TO_CLOSE, S_CLOSE);
        AddState(sPlay);

        var sClose = new FSMState(S_CLOSE);
        sClose.AddTransition(I_TO_INIT, S_INIT);
        AddState(sClose);
    }
}
