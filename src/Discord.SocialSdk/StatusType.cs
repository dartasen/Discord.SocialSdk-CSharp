namespace Discord.SocialSdk;

/// <summary>
///  Enum that specifies the various online statuses for a user.
/// </summary>
/// <remarks>
///  Generally a user is online or offline, but in Discord users are able to further customize their
///  status such as turning on "Do not Disturb" mode or "Dnd" to silence notifications.
///
/// </remarks>
public enum StatusType
{
    Online = 0,
    Offline = 1,
    Blocked = 2,
    Idle = 3,
    Dnd = 4,
    Invisible = 5,
    Streaming = 6,
    Unknown = 7,
}
