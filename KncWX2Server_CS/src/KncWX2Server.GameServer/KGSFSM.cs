using KncWX2Server.Core.Framework;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/GSFSM.{h,cpp} (8 states + all transitions).
/// </summary>
public sealed class KGSFSM : FSMClass
{
    public const int S_EXIT = 0;
    public const int S_INIT = 1;
    public const int S_UID_UPDATED = 2;
    public const int S_LOGINED = 3;
    public const int S_CHECK_SECOND_PW = 4;
    public const int S_SERVER_SELECT = 5;
    public const int S_FIELD_MAP = 6;
    public const int S_ROOM = 7;

    public const int I_TO_UID_UPDATED = 1;
    public const int I_TO_LOGINED = 2;
    public const int I_TO_CHECK_SECOND_PW = 3;
    public const int I_TO_SERVER_SELECT = 4;
    public const int I_TO_FIELD_MAP = 5;
    public const int I_TO_ROOM = 6;
    public const int I_TO_INIT = 7;
    public const int I_TO_EXIT = 99;

    public KGSFSM() : base(S_INIT)
    {
        var sInit = new FSMState(S_INIT);
        sInit.AddTransition(I_TO_UID_UPDATED, S_UID_UPDATED);
        sInit.AddTransition(I_TO_LOGINED, S_LOGINED);
        sInit.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sInit);

        var sUid = new FSMState(S_UID_UPDATED);
        sUid.AddTransition(I_TO_LOGINED, S_LOGINED);
        sUid.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sUid);

        var sLogined = new FSMState(S_LOGINED);
        sLogined.AddTransition(I_TO_CHECK_SECOND_PW, S_CHECK_SECOND_PW);
        sLogined.AddTransition(I_TO_SERVER_SELECT, S_SERVER_SELECT);
        sLogined.AddTransition(I_TO_FIELD_MAP, S_FIELD_MAP);
        sLogined.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sLogined);

        var sSecPw = new FSMState(S_CHECK_SECOND_PW);
        sSecPw.AddTransition(I_TO_SERVER_SELECT, S_SERVER_SELECT);
        sSecPw.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sSecPw);

        var sSel = new FSMState(S_SERVER_SELECT);
        sSel.AddTransition(I_TO_FIELD_MAP, S_FIELD_MAP);
        sSel.AddTransition(I_TO_CHECK_SECOND_PW, S_CHECK_SECOND_PW);
        sSel.AddTransition(I_TO_ROOM, S_ROOM);
        sSel.AddTransition(I_TO_LOGINED, S_LOGINED);
        sSel.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sSel);

        var sField = new FSMState(S_FIELD_MAP);
        sField.AddTransition(I_TO_ROOM, S_ROOM);
        sField.AddTransition(I_TO_SERVER_SELECT, S_SERVER_SELECT);
        sField.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sField);

        var sRoom = new FSMState(S_ROOM);
        sRoom.AddTransition(I_TO_FIELD_MAP, S_FIELD_MAP);
        sRoom.AddTransition(I_TO_SERVER_SELECT, S_SERVER_SELECT);
        sRoom.AddTransition(I_TO_EXIT, S_EXIT);
        AddState(sRoom);

        var sExit = new FSMState(S_EXIT);
        sExit.AddTransition(I_TO_INIT, S_INIT);
        AddState(sExit);
    }
}
