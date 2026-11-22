using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Port of Common/GameSysVal/GameSysVal.{h,cpp} and Common/SMSPhoneNumberManager.{h,cpp}.
/// </summary>
public sealed class KGameSysVal
{
    public int LimitsMaxLevel { get; private set; } = 65;
    public double PremiumExp { get; private set; } = 0.3;
    public double VipExp { get; private set; } = 0.05;
    public int GuestUserLimitLevel { get; private set; } = 19;
    public bool IsHackUserRegRejected { get; private set; }
    public bool IsHackUserRegMonitors { get; private set; } = true;
    public bool IsHackUserKick { get; private set; }
    public bool CheckDropCcu { get; private set; }
    public float DropCcuPercent { get; private set; } = 0.2f;
    public bool ProcessCommunication { get; private set; }
    public bool MachineIdDuplicateCheck { get; private set; }

    public void AddLimitsMaxLevel(int level) => LimitsMaxLevel = level;
    public void AddPremiumEXP(double rate) => PremiumExp = rate;
    public void AddVIPEXP(double rate) => VipExp = rate;
    public void SetGuestUserLimitLevel(int level) => GuestUserLimitLevel = level;
    public void SetIsHackUserRegRejected(bool val) => IsHackUserRegRejected = val;
    public void SetIsHackUserRegMonitors(bool val) => IsHackUserRegMonitors = val;
    public void SetIsHackUserKick(bool val) => IsHackUserKick = val;
    public void SetCheckDropCCU(bool val) => CheckDropCcu = val;
    public void SetDropCCUPercent(float pct) => DropCcuPercent = pct;
    public void SetProcessCommunication(bool val) => ProcessCommunication = val;
    public void SetMachineIDDuplicateCheck(bool val) => MachineIdDuplicateCheck = val;
    public void AddKNXGameBang(double a, double b, double c, double d) { }
    public void AddKNXGameBangPetEXP(double rate) { }
    public void AddPCBang(int t, double e, double ed, double vs, bool sp, double pet, int he, bool sec, bool av, bool ti) { }

    public void BindToLua(KScriptManager script, string globalName = "GameSysVal")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddLimitsMaxLevel"] = DynValue.NewCallback((_, a) => { AddLimitsMaxLevel(KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), LimitsMaxLevel)); return DynValue.Nil; });
            t["AddPremiumEXP"] = DynValue.NewCallback((_, a) => { AddPremiumEXP(KScriptManager.ArgDouble(a, KScriptManager.ArgStart(a, proxy!), PremiumExp)); return DynValue.Nil; });
            t["AddVIPEXP"] = DynValue.NewCallback((_, a) => { AddVIPEXP(KScriptManager.ArgDouble(a, KScriptManager.ArgStart(a, proxy!), VipExp)); return DynValue.Nil; });
            t["SetGuestUserLimitLevel"] = DynValue.NewCallback((_, a) => { SetGuestUserLimitLevel(KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), GuestUserLimitLevel)); return DynValue.Nil; });
            t["SetIsHackUserRegRejected"] = DynValue.NewCallback((_, a) => { SetIsHackUserRegRejected(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsHackUserRegRejected)); return DynValue.Nil; });
            t["SetIsHackUserRegMonitors"] = DynValue.NewCallback((_, a) => { SetIsHackUserRegMonitors(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsHackUserRegMonitors)); return DynValue.Nil; });
            t["SetIsHackUserKick"] = DynValue.NewCallback((_, a) => { SetIsHackUserKick(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsHackUserKick)); return DynValue.Nil; });
            t["SetCheckDropCCU"] = DynValue.NewCallback((_, a) => { SetCheckDropCCU(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), CheckDropCcu)); return DynValue.Nil; });
            t["SetDropCCUPercent"] = DynValue.NewCallback((_, a) => { SetDropCCUPercent(KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), DropCcuPercent)); return DynValue.Nil; });
            t["SetProcessCommunication"] = DynValue.NewCallback((_, a) => { SetProcessCommunication(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), ProcessCommunication)); return DynValue.Nil; });
            t["SetMachineIDDuplicateCheck"] = DynValue.NewCallback((_, a) => { SetMachineIDDuplicateCheck(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), MachineIdDuplicateCheck)); return DynValue.Nil; });
        });
    }
}

public sealed class KSMSPhoneNumberManager
{
    private readonly Dictionary<int, List<string>> _numbers = new();
    private readonly Dictionary<int, List<int>> _links = new();

    public void AddPhoneNumber(int type, string phone)
    {
        if (!_numbers.TryGetValue(type, out var list))
        {
            list = [];
            _numbers[type] = list;
        }
        if (!list.Contains(phone)) list.Add(phone);
    }

    public void AddTeamPhoneNumber(int type, string phone) => AddPhoneNumber(type, phone);
    public void AddTeamPhoneNumber_LUA(int type, string phone) => AddPhoneNumber(type, phone);
    public void Link(int smsType, int teamType)
    {
        if (!_links.TryGetValue(smsType, out var list))
        {
            list = [];
            _links[smsType] = list;
        }
        if (!list.Contains(teamType)) list.Add(teamType);
    }

    public List<string> GetPhoneNumberList(int type)
        => _numbers.TryGetValue(type, out var list) ? list.ToList() : [];

    public void BindToLua(KScriptManager script, string globalName = "g_SMSPhoneNumberManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddPhoneNumber"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddPhoneNumber(KScriptManager.ArgInt(a, o), KScriptManager.ArgString(a, o + 1));
                return DynValue.Nil;
            });
            t["AddTeamPhoneNumber"] = t["AddPhoneNumber"];
            t["AddTeamPhoneNumber_LUA"] = t["AddPhoneNumber"];
            t["AddFuntionalPhoneNumber"] = t["AddPhoneNumber"];
            t["Link"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                Link(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
        });
    }
}
