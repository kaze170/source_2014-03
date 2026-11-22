using KncWX2Server.Core.Packets;

namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Port of ChannelServer/CCUToolManager.{h,cpp}.
/// </summary>
public sealed class KCCUToolManager
{
    public bool CheckLogin(string id, string pw)
    {
        return string.Equals(id, "kogccutool", StringComparison.Ordinal)
            && string.Equals(pw, "rhdzja980205", StringComparison.Ordinal);
    }

    public void UpdateServerCCU(Dictionary<int, KChannelInfo> channelList, List<KServerInfo> serverList)
    {
        foreach (var server in serverList)
        {
            foreach (var ch in channelList.Values)
            {
                if (string.Equals(ch.Ip, server.Ip, StringComparison.OrdinalIgnoreCase) &&
                    ch.MasterPort == server.MasterPort)
                {
                    ch.CurrentUser = server.MaxUser;
                }
            }
        }
    }
}
