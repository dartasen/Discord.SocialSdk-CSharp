namespace Discord.SocialSdk;

/// <summary>
///  Represents the type of auth token used by the SDK, either the normal tokens produced by the
///  Discord desktop app, or an oauth2 bearer token. Only the latter can be used by the SDK.
/// </summary>
public enum AuthorizationTokenType
{
    User = 0,
    Bearer = 1,
}
