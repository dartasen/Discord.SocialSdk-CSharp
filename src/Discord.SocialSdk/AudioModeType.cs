namespace Discord.SocialSdk;

/// <summary>
///  Represents whether a voice call is using push to talk or auto voice detection
/// </summary>
public enum AudioModeType
{
    MODE_UNINIT = 0,
    MODE_VAD = 1,
    MODE_PTT = 2,
}
