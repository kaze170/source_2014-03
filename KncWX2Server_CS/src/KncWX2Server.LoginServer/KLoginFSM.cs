using KncWX2Server.Core.Framework;

namespace KncWX2Server.LoginServer;

public sealed class KLoginFSM : FSMClass
{
    public const int S_EXIT = 0;
    public const int S_INIT = 1;
    public const int S_CHECK_SECOND_PW = 2;
    public const int S_LOGINED = 3;
    public const int S_SERVER_SELECT = 4;

    public const int I_TO_CHECK_SECOND_PW = 1;
    public const int I_TO_LOGINED = 2;
    public const int I_TO_SERVER_SELECT = 3;
    public const int I_TO_EXIT = 99;

    public KLoginFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_CHECK_SECOND_PW, S_CHECK_SECOND_PW);
        sInit.AddTransition(I_TO_LOGINED, S_LOGINED);
        sInit.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sInit);

        var sSecPw = new FSMState(S_CHECK_SECOND_PW);
        sSecPw.AddTransition(I_TO_LOGINED, S_LOGINED);
        sSecPw.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sSecPw);

        var sLogined = new FSMState(S_LOGINED);
        sLogined.AddTransition(I_TO_SERVER_SELECT, S_SERVER_SELECT);
        sLogined.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sLogined);

        var sSel = new FSMState(S_SERVER_SELECT);
        sSel.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sSel);

        AddState(new FSMState(S_EXIT));
    }
}
