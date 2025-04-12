namespace Discord.SocialSdk;

/// <summary>
///  Represents the crypto method used to generate a code challenge.
/// </summary>
/// <remarks>
///  The only method used by the SDK is sha256.
///
/// </remarks>
public enum AuthenticationCodeChallengeMethod
{
    S256 = 0,
}
