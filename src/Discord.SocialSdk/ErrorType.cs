namespace Discord.SocialSdk;

/// <summary>
///  Enum representing various types of errors the SDK returns.
/// </summary>
public enum ErrorType
{
    None = 0,
    NetworkError = 1,
    HTTPError = 2,
    ClientNotReady = 3,
    Disabled = 4,
    ClientDestroyed = 5,
    ValidationError = 6,
    Aborted = 7,
    AuthorizationFailed = 8,
    RPCError = 9,
}
