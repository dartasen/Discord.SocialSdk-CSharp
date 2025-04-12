namespace Discord.SocialSdk;

/// <summary>
///  Represents the various identity providers that can be used to authenticate a provisional
///  account user for public clients.
/// </summary>
public enum AuthenticationExternalAuthType
{
    OIDC = 0,
    EpicOnlineServicesAccessToken = 1,
    EpicOnlineServicesIdToken = 2,
    SteamSessionTicket = 3,
    UnityServicesIdToken = 4,
}
