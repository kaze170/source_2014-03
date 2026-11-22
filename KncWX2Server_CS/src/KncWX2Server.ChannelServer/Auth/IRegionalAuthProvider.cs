using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.Auth;

/// <summary>
/// Abstraction over regional publisher authentication (TW/HK GASH+, CN Giant Login, JP NHN Purple).
/// </summary>
public interface IRegionalAuthProvider
{
    RegionCode[] SupportedRegions { get; }

    void RequestPublisherLogin(KChannelUser user, KECH_VERIFY_ACCOUNT_REQ req);

    bool ShouldKeepAliveOnFailure(int netError);

    void RequestPublisherSecurityAuth(KChannelUser user, KECH_SECURITY_AUTH_REQ req);

    void HandlePublisherSecurityInfoNot(KChannelUser user, KEPUBLISHER_SECURITY_INFO_NOT info);
}
