namespace Discord.SocialSdk;

/// <summary>
///  Discord RichPresence supports multiple types of activities that a user can be doing.
/// </summary>
/// <remarks>
///  For the SDK, the only activity type that is really relevant is `Playing`.
///  The others are provided for completeness.
///
///  See https://discord.com/developers/docs/rich-presence/overview for more information.
///
/// </remarks>
public enum ActivityTypes
{
    Playing = 0,
    Streaming = 1,
    Listening = 2,
    Watching = 3,
    CustomStatus = 4,
    Competing = 5,
    HangStatus = 6,
}
