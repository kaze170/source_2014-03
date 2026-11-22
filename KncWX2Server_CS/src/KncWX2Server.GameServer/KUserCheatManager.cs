namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserCheatManager.{h,cpp} (SERV_OBSERVER_PVP_INTRUDE_MODE):
/// manages GM/observer PvP intrusion mode state per session.
/// </summary>
public sealed class KUserCheatManager
{
    public bool ObserverMode { get; set; }
}
