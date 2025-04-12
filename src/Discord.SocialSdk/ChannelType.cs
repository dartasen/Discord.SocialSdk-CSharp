namespace Discord.SocialSdk;

/// <summary>
///  Enum that represents the various channel types on Discord.
/// </summary>
/// <remarks>
///  For more information see: https://discord.com/developers/docs/resources/channel
///
/// </remarks>
public enum ChannelType
{
    GuildText = 0,
    Dm = 1,
    GuildVoice = 2,
    GroupDm = 3,
    GuildCategory = 4,
    GuildNews = 5,
    GuildStore = 6,
    GuildNewsThread = 10,
    GuildPublicThread = 11,
    GuildPrivateThread = 12,
    GuildStageVoice = 13,
    GuildDirectory = 14,
    GuildForum = 15,
    GuildMedia = 16,
    Lobby = 17,
    EphemeralDm = 18,
}
