using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  The Client class is the main entry point for the Discord SDK. All functionality is exposed
///  through this class.
/// </summary>
/// <remarks>
///  See @ref getting_started "Getting Started" for more information on how to use the Client class.
///
/// </remarks>
public class Client : IDisposable
{
    internal NativeMethods.Client self;
    private int disposed_;

    internal Client(NativeMethods.Client self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~Client() { Dispose(); }

    /// <summary>
    ///  Represents an error state for the socket connection that the Discord SDK maintains with the
    ///  Discord backend.
    /// </summary>
    /// <remarks>
    ///  Generic network failures will use the ConnectionFailed and ConnectionCanceled
    ///  enum values. Other errors such as if the user's auth token is invalid or out of
    ///  date will be UnexpectedClose and you should look at the other Error fields for the specific
    ///  details.
    ///
    /// </remarks>
    public enum Error
    {
        None = 0,
        ConnectionFailed = 1,
        UnexpectedClose = 2,
        ConnectionCanceled = 3,
    }

    /// <summary>
    ///  This enum refers to the status of the internal websocket the SDK uses to communicate with
    ///  Discord There are ~2 phases for "launching" the client:
    ///  1. The socket has to connect to Discord and exchange an auth token. This is indicated by
    ///  the `Connecting` and `Connected` values.
    ///  2. The socket has to receive an initial payload of data that describes the current user,
    ///  what lobbies they are in, who their friends are, etc. This is the `Ready` status.
    ///  Many Client functions will not work until the status changes to `Ready`, such as
    ///  GetCurrentUser().
    /// </summary>
    /// <remarks>
    ///  Status::Ready is the one you want to wait for!
    ///
    ///  Additionally, sometimes the socket will be disconnected, such as through temporary network
    ///  blips. But it will try to automatically reconnect, as indicated by the `Reconnecting`
    ///  status.
    ///
    /// </remarks>
    public enum Status
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Ready = 3,
        Reconnecting = 4,
        Disconnecting = 5,
        HttpWait = 6,
    }

    /// <summary>
    ///  Represents the type of thread to control thread priority on.
    /// </summary>
    public enum Thread
    {
        Client = 0,
        Voice = 1,
        Network = 2,
    }

    public delegate void EndCallCallback();
    public delegate void EndCallsCallback();
    public delegate void GetCurrentInputDeviceCallback(AudioDevice device);
    public delegate void GetCurrentOutputDeviceCallback(AudioDevice device);
    public delegate void GetInputDevicesCallback(AudioDevice[] devices);
    public delegate void GetOutputDevicesCallback(AudioDevice[] devices);
    public delegate void DeviceChangeCallback(AudioDevice[] inputDevices,
                                              AudioDevice[] outputDevices);
    public delegate void SetInputDeviceCallback(ClientResult result);
    public delegate void NoAudioInputCallback(bool inputDetected);
    public delegate void SetOutputDeviceCallback(ClientResult result);
    public delegate void VoiceParticipantChangedCallback(ulong lobbyId, ulong memberId, bool added);
    public delegate void UserAudioReceivedCallback(ulong userId,
                                                   IntPtr data,
                                                   ulong samplesPerChannel,
                                                   int sampleRate,
                                                   ulong channels,
                                                   ref bool outShouldMute);
    public delegate void UserAudioCapturedCallback(IntPtr data,
                                                   ulong samplesPerChannel,
                                                   int sampleRate,
                                                   ulong channels);
    public delegate void AuthorizationCallback(ClientResult result,
                                               string code,
                                               string redirectUri);
    public delegate void FetchCurrentUserCallback(ClientResult result,
                                                  ulong id,
                                                  string name);
    public delegate void TokenExchangeCallback(ClientResult result,
                                               string accessToken,
                                               string refreshToken,
                                               AuthorizationTokenType tokenType,
                                               int expiresIn,
                                               string scopes);
    public delegate void AuthorizeDeviceScreenClosedCallback();
    public delegate void TokenExpirationCallback();
    public delegate void UpdateProvisionalAccountDisplayNameCallback(
      ClientResult result);
    public delegate void UpdateTokenCallback(ClientResult result);
    public delegate void DeleteUserMessageCallback(ClientResult result);
    public delegate void EditUserMessageCallback(ClientResult result);
    public delegate void ProvisionalUserMergeRequiredCallback();
    public delegate void OpenMessageInDiscordCallback(ClientResult result);
    public delegate void SendUserMessageCallback(ClientResult result, ulong messageId);
    public delegate void MessageCreatedCallback(ulong messageId);
    public delegate void MessageDeletedCallback(ulong messageId, ulong channelId);
    public delegate void MessageUpdatedCallback(ulong messageId);
    public delegate void LogCallback(string message, LoggingSeverity severity);
    public delegate void OnStatusChanged(Status status,
                                         Error error,
                                         int errorDetail);
    public delegate void CreateOrJoinLobbyCallback(ClientResult result, ulong lobbyId);
    public delegate void GetGuildChannelsCallback(ClientResult result,
                                                  GuildChannel[] guildChannels);
    public delegate void GetUserGuildsCallback(ClientResult result,
                                               GuildMinimal[] guilds);
    public delegate void LeaveLobbyCallback(ClientResult result);
    public delegate void LinkOrUnlinkChannelCallback(ClientResult result);
    public delegate void LobbyCreatedCallback(ulong lobbyId);
    public delegate void LobbyDeletedCallback(ulong lobbyId);
    public delegate void LobbyMemberAddedCallback(ulong lobbyId, ulong memberId);
    public delegate void LobbyMemberRemovedCallback(ulong lobbyId, ulong memberId);
    public delegate void LobbyMemberUpdatedCallback(ulong lobbyId, ulong memberId);
    public delegate void LobbyUpdatedCallback(ulong lobbyId);
    public delegate void AcceptActivityInviteCallback(ClientResult result,
                                                      string joinSecret);
    public delegate void SendActivityInviteCallback(ClientResult result);
    public delegate void ActivityInviteCallback(ActivityInvite invite);
    public delegate void ActivityJoinCallback(string joinSecret);
    public delegate void UpdateStatusCallback(ClientResult result);
    public delegate void UpdateRichPresenceCallback(ClientResult result);
    public delegate void UpdateRelationshipCallback(ClientResult result);
    public delegate void SendFriendRequestCallback(ClientResult result);
    public delegate void RelationshipCreatedCallback(ulong userId,
                                                     bool isDiscordRelationshipUpdate);
    public delegate void RelationshipDeletedCallback(ulong userId,
                                                     bool isDiscordRelationshipUpdate);
    public delegate void GetDiscordClientConnectedUserCallback(ClientResult result,
                                                               UserHandle? user);
    public delegate void UserUpdatedCallback(ulong userId);
    /// <summary>
    ///  Creates a new instance of the Client.
    /// </summary>
    public Client()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.Init(self);
            }
        }
    }
    /// <summary>
    ///  Creates a new instance of the Client but allows customizing the Discord URL to use.
    /// </summary>
    public Client(string apiBase, string webBase)
    {
        NativeMethods.__Init();
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __apiBaseSpan;
            var __apiBaseOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__apiBaseSpan, apiBase);
            NativeMethods.Discord_String __webBaseSpan;
            var __webBaseOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__webBaseSpan, webBase);
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.InitWithBases(self, __apiBaseSpan, __webBaseSpan);
            }
            NativeMethods.__FreeLocal(&__webBaseSpan, __webBaseOwned);
            NativeMethods.__FreeLocal(&__apiBaseSpan, __apiBaseOwned);
        }
    }
    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.Drop(self);
            }
        }
    }
    /// <summary>
    ///  Converts the Error enum to a string.
    /// </summary>
    public static string ErrorToString(Error type)
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.ErrorToString(type, &__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  This function is used to get the application ID for the client. This is used to
    ///  identify the application to the Discord client. This is used for things like
    ///  authentication, rich presence, and activity invites when *not* connected with
    ///  Client::Connect. When calling Client::Connect, the application ID is set automatically
    /// </summary>
    public ulong GetApplicationId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetApplicationId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ID of the system default audio device if the user has not explicitly chosen
    ///  one.
    /// </summary>
    public static string GetDefaultAudioDeviceId()
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.GetDefaultAudioDeviceId(&__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the default set of OAuth2 scopes that should be used with the Discord SDK
    ///  when making use of the full SDK capabilities, including communications-related features
    ///  (e.g. user DMs, lobbies, voice chat). If your application does not make use of these
    ///  features, you should use Client::GetDefaultPresenceScopes instead.
    /// </summary>
    /// <remarks>
    ///  Communications-related features are currently in limited access and are not available to
    ///  all applications, however, they can be demoed in limited capacity by all applications. If
    ///  you are interested in using these features in your game, please reach out to the Discord
    ///  team.
    ///
    ///  It's ok to further customize your requested oauth2 scopes to add additional scopes if you
    ///  have legitimate usages for them. However, we strongly recommend always using
    ///  Client::GetDefaultCommunicationScopes or Client::GetDefaultPresenceScopes as a baseline to
    ///  enable a better authorization experience for users!
    ///
    /// </remarks>
    public static string GetDefaultCommunicationScopes()
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.GetDefaultCommunicationScopes(&__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the default set of OAuth2 scopes that should be used with the Discord SDK
    ///  when leveraging baseline presence-related features (e.g. friends list, rich presence,
    ///  provisional accounts, activity invites). If your application is using
    ///  communications-related features, which are currently available in limited access, you
    ///  should use Client::GetDefaultCommunicationScopes instead.
    /// </summary>
    /// <remarks>
    ///  It's ok to further customize your requested oauth2 scopes to add additional scopes if you
    ///  have legitimate usages for them. However, we strongly recommend always using
    ///  Client::GetDefaultCommunicationScopes or Client::GetDefaultPresenceScopes as a baseline to
    ///  enable a better authorization experience for users!
    ///
    /// </remarks>
    public static string GetDefaultPresenceScopes()
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.GetDefaultPresenceScopes(&__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the git commit hash this version was built from.
    /// </summary>
    public static string GetVersionHash()
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.GetVersionHash(&__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the major version of the Discord Social SDK.
    /// </summary>
    public static int GetVersionMajor()
    {
        unsafe
        {
            int __returnValue;
            __returnValue = NativeMethods.Client.GetVersionMajor();
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the minor version of the Discord Social SDK.
    /// </summary>
    public static int GetVersionMinor()
    {
        unsafe
        {
            int __returnValue;
            __returnValue = NativeMethods.Client.GetVersionMinor();
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the patch version of the Discord Social SDK.
    /// </summary>
    public static int GetVersionPatch()
    {
        unsafe
        {
            int __returnValue;
            __returnValue = NativeMethods.Client.GetVersionPatch();
            return __returnValue;
        }
    }
    /// <summary>
    ///  Converts the Status enum to a string.
    /// </summary>
    public static string StatusToString(Status type)
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.StatusToString(type, &__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Converts the Thread enum to a string.
    /// </summary>
    public static string ThreadToString(Thread type)
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.Client.ThreadToString(type, &__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Ends any active call, if any. Any references you have to Call objects are invalid after
    ///  they are ended, and can be immediately freed.
    /// </summary>
    public void EndCall(ulong channelId, EndCallCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.EndCallCallback __callbackDelegate =
          NativeMethods.Client.EndCallCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.EndCall(self,
                                             channelId,
                                             __callbackDelegate,
                                             NativeMethods.ManagedUserData.Free,
                                             NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Ends any active call, if any. Any references you have to Call objects are invalid after
    ///  they are ended, and can be immediately freed.
    /// </summary>
    public void EndCalls(EndCallsCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.EndCallsCallback __callbackDelegate =
          NativeMethods.Client.EndCallsCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.EndCalls(self,
                                              __callbackDelegate,
                                              NativeMethods.ManagedUserData.Free,
                                              NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Returns a reference to the currently active call, if any.
    /// </summary>
    public Call? GetCall(ulong channelId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.Call();
            Call? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.GetCall(self, channelId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new Call(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a reference to all currently active calls, if any.
    /// </summary>
    public Call?[] GetCalls()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_CallSpan();
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetCalls(self, &__returnValue);
            }
            var __returnValueSurface = new Call?[(int)__returnValue.size];
            for (int __i = 0; __i < (int)__returnValue.size; __i++)
            {
                __returnValueSurface[__i] = new Call(__returnValue.ptr[__i], 0);
            }
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Asynchronously fetches the current audio input device in use by the client.
    /// </summary>
    public void GetCurrentInputDevice(GetCurrentInputDeviceCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetCurrentInputDeviceCallback __cbDelegate =
          NativeMethods.Client.GetCurrentInputDeviceCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetCurrentInputDevice(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Asynchronously fetches the current audio output device in use by the client.
    /// </summary>
    public void GetCurrentOutputDevice(GetCurrentOutputDeviceCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetCurrentOutputDeviceCallback __cbDelegate =
          NativeMethods.Client.GetCurrentOutputDeviceCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetCurrentOutputDevice(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Asynchronously fetches the list of audio input devices available to the user.
    /// </summary>
    public void GetInputDevices(GetInputDevicesCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetInputDevicesCallback __cbDelegate =
          NativeMethods.Client.GetInputDevicesCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetInputDevices(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Returns the input volume for the current user's microphone.
    /// </summary>
    /// <remarks>
    ///  Input volume is specified as a percentage in the range [0, 100] which represents the
    ///  perceptual loudness.
    ///
    /// </remarks>
    public float GetInputVolume()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            float __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetInputVolume(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Asynchronously fetches the list of audio output devices available to the user.
    /// </summary>
    public void GetOutputDevices(GetOutputDevicesCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetOutputDevicesCallback __cbDelegate =
          NativeMethods.Client.GetOutputDevicesCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetOutputDevices(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Returns the output volume for the current user.
    /// </summary>
    /// <remarks>
    ///  Output volume specified as a percentage in the range [0, 200] which represents the
    ///  perceptual loudness.
    ///
    /// </remarks>
    public float GetOutputVolume()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            float __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetOutputVolume(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether the current user is deafened in all calls.
    /// </summary>
    public bool GetSelfDeafAll()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetSelfDeafAll(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether the current user's microphone is muted in all calls.
    /// </summary>
    public bool GetSelfMuteAll()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetSelfMuteAll(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  When enabled, automatically adjusts the microphone volume to keep it clear and consistent.
    /// </summary>
    /// <remarks>
    ///  Defaults to on.
    ///
    ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
    ///  the user to control, similar to Discord's voice settings.
    ///
    /// </remarks>
    public void SetAutomaticGainControl(bool on)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetAutomaticGainControl(self, on);
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked when Discord detects a change in the available audio
    ///  devices.
    /// </summary>
    public void SetDeviceChangeCallback(DeviceChangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.DeviceChangeCallback __callbackDelegate =
          NativeMethods.Client.DeviceChangeCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetDeviceChangeCallback(
                  self,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Enables or disables the basic echo cancellation provided by the WebRTC library.
    /// </summary>
    /// <remarks>
    ///  Defaults to on.
    ///
    ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
    ///  the user to control, similar to Discord's voice settings.
    ///
    /// </remarks>
    public void SetEchoCancellation(bool on)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetEchoCancellation(self, on);
            }
        }
    }
    /// <summary>
    ///  Asynchronously changes the audio input device in use by the client to the specified device.
    ///  You can find the list of device IDs that can be passed in with the Client::GetInputDevices
    ///  function.
    /// </summary>
    public void SetInputDevice(string deviceId, SetInputDeviceCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __deviceIdSpan;
            var __deviceIdOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__deviceIdSpan, deviceId);
            NativeMethods.Client.SetInputDeviceCallback __cbDelegate =
          NativeMethods.Client.SetInputDeviceCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetInputDevice(self,
                                                    __deviceIdSpan,
                                                    __cbDelegate,
                                                    NativeMethods.ManagedUserData.Free,
                                                    NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__deviceIdSpan, __deviceIdOwned);
        }
    }
    /// <summary>
    ///  Sets the microphone volume for the current user.
    /// </summary>
    /// <remarks>
    ///  Input volume is specified as a percentage in the range [0, 100] which represents the
    ///  perceptual loudness.
    ///
    /// </remarks>
    public void SetInputVolume(float inputVolume)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetInputVolume(self, inputVolume);
            }
        }
    }
    /// <summary>
    ///  Callback function invoked when the above threshold is set and there is a change in whether
    ///  audio is being detected.
    /// </summary>
    public void SetNoAudioInputCallback(NoAudioInputCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.NoAudioInputCallback __callbackDelegate =
          NativeMethods.Client.NoAudioInputCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetNoAudioInputCallback(
                  self,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Threshold that can be set to indicate when no audio is being received by the user's mic.
    /// </summary>
    /// <remarks>
    ///  An example of when this may be useful: When push to talk is being used and the user pushes
    ///  their talk key, but something is configured wrong and no audio is being received, this
    ///  threshold and callback can be used to detect that situation and notify the user. The
    ///  threshold is specified in DBFS, or decibels relative to full scale, and the range is
    ///  [-100.0, 100.0] It defaults to -100.0, so is disabled.
    ///
    /// </remarks>
    public void SetNoAudioInputThreshold(float dBFSThreshold)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetNoAudioInputThreshold(self, dBFSThreshold);
            }
        }
    }
    /// <summary>
    ///  Enables basic background noise suppression.
    /// </summary>
    /// <remarks>
    ///  Defaults to on.
    ///
    ///  Generally this shouldn't need to be used unless you are building a voice settings UI for
    ///  the user to control, similar to Discord's voice settings.
    ///
    /// </remarks>
    public void SetNoiseSuppression(bool on)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetNoiseSuppression(self, on);
            }
        }
    }
    /// <summary>
    ///  Enables or disables hardware encoding and decoding for audio, if it is available.
    /// </summary>
    /// <remarks>
    ///  Defaults to on.
    ///
    ///  This must be called immediately after constructing the Client. If called too late an error
    ///  will be logged and the setting will not take effect.
    ///
    /// </remarks>
    public void SetOpusHardwareCoding(bool encode, bool decode)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetOpusHardwareCoding(self, encode, decode);
            }
        }
    }
    /// <summary>
    ///  Asynchronously changes the audio output device in use by the client to the specified
    ///  device. You can find the list of device IDs that can be passed in with the
    ///  Client::GetOutputDevices function.
    /// </summary>
    public void SetOutputDevice(string deviceId, SetOutputDeviceCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __deviceIdSpan;
            var __deviceIdOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__deviceIdSpan, deviceId);
            NativeMethods.Client.SetOutputDeviceCallback __cbDelegate =
          NativeMethods.Client.SetOutputDeviceCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetOutputDevice(
                  self,
                  __deviceIdSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__deviceIdSpan, __deviceIdOwned);
        }
    }
    /// <summary>
    ///  Sets the speaker volume for the current user.
    /// </summary>
    /// <remarks>
    ///  Output volume specified as a percentage in the range [0, 200] which represents the
    ///  perceptual loudness.
    ///
    /// </remarks>
    public void SetOutputVolume(float outputVolume)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetOutputVolume(self, outputVolume);
            }
        }
    }
    /// <summary>
    ///  Mutes all audio from the currently active call for the current user in all calls.
    ///  They will not be able to hear any other participants,
    ///  and no other participants will be able to hear the current user either.
    ///  Note: This overrides the per-call setting.
    /// </summary>
    public void SetSelfDeafAll(bool deaf)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetSelfDeafAll(self, deaf);
            }
        }
    }
    /// <summary>
    ///  Mutes the current user's microphone so that no other participant in their active calls can
    ///  hear them in all calls.
    ///  Note: This overrides the per-call setting.
    /// </summary>
    public void SetSelfMuteAll(bool mute)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetSelfMuteAll(self, mute);
            }
        }
    }
    /// <summary>
    ///  On mobile devices, enable speakerphone mode.
    /// </summary>
    public bool SetSpeakerMode(bool speakerMode)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.SetSpeakerMode(self, speakerMode);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Allows setting the priority of various SDK threads.
    /// </summary>
    /// <remarks>
    ///  The threads that can be controlled are:
    ///  - Client: This is the main thread for the SDK where most of the data processing happens
    ///  - Network: This is the thread that receives voice data from lobby calls
    ///  - Voice: This is the thread that the voice engine runs on and processes all audio data
    ///
    /// </remarks>
    public void SetThreadPriority(Thread thread, int priority)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetThreadPriority(self, thread, priority);
            }
        }
    }
    /// <summary>
    ///  Callback invoked whenever a user in a lobby joins or leaves a voice call.
    /// </summary>
    /// <remarks>
    ///  The main use case for this is to enable displaying which users are in voice in a lobby
    ///  even if the current user is not in voice yet, and thus does not have a Call object to bind
    ///  to.
    ///
    /// </remarks>
    public void SetVoiceParticipantChangedCallback(
      VoiceParticipantChangedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.VoiceParticipantChangedCallback __cbDelegate =
          NativeMethods.Client.VoiceParticipantChangedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetVoiceParticipantChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  On iOS devices, show the system audio route picker.
    /// </summary>
    public bool ShowAudioRoutePicker()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.ShowAudioRoutePicker(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Starts or joins a call in the lobby specified by channelId (For a lobby, simply
    ///  pass in the lobbyId).
    /// </summary>
    /// <remarks>
    ///  On iOS, your application is responsible for enabling the appropriate background audio mode
    ///  in your Info.plist. VoiceBuildPostProcessor in the sample demonstrates how to do this
    ///  automatically in your Unity build process.
    ///
    ///  On macOS, you should set the NSMicrophoneUsageDescription key in your Info.plist.
    ///
    ///  Returns null if the user is already in the given voice channel.
    ///
    /// </remarks>
    public Call? StartCall(ulong channelId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.Call();
            Call? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.StartCall(self, channelId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new Call(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Starts or joins a call in the specified lobby.
    /// </summary>
    /// <remarks>
    ///  The audio received callback is invoked whenever incoming audio is received in a call. If
    ///  the developer sets outShouldMute to true during the callback, the audio data will be muted
    ///  after the callback is invoked, which is useful if the developer is utilizing the incoming
    ///  audio and playing it through their own audio engine or playback. The audio samples
    ///  in `data` can be modified in-place for simple DSP effects.
    ///
    ///  The audio captured callback is invoked whenever local audio is captured before it is
    ///  processed and transmitted which may be useful for voice moderation, etc.
    ///
    ///  On iOS, your application is responsible for enabling the appropriate background audio mode
    ///  in your Info.plist. VoiceBuildPostProcessor in the sample demonstrates how to do this
    ///  automatically in your Unity build process.
    ///
    ///  On macOS, you should set the NSMicrophoneUsageDescription key in your Info.plist.
    ///
    ///  Returns null if the user is already in the given voice channel.
    ///
    /// </remarks>
    public Call? StartCallWithAudioCallbacks(
      ulong lobbyId,
      UserAudioReceivedCallback receivedCb,
      UserAudioCapturedCallback capturedCb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.Call();
            Call? __returnValue = null;
            NativeMethods.Client.UserAudioReceivedCallback __receivedCbDelegate =
          NativeMethods.Client.UserAudioReceivedCallback_Handler;
            NativeMethods.Client.UserAudioCapturedCallback __capturedCbDelegate =
          NativeMethods.Client.UserAudioCapturedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Client.StartCallWithAudioCallbacks(
                  self,
                  lobbyId,
                  __receivedCbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(receivedCb),
                  __capturedCbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(capturedCb),
                  &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new Call(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  This will abort the authorize flow if it is in progress and tear down any associated state.
    /// </summary>
    /// <remarks>
    ///  NOTE: this *will not* close authorization windows presented to the user.
    ///
    /// </remarks>
    public void AbortAuthorize()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AbortAuthorize(self);
            }
        }
    }
    /// <summary>
    ///  This function is used to abort/cleanup the device authorization flow.
    /// </summary>
    public void AbortGetTokenFromDevice()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AbortGetTokenFromDevice(self);
            }
        }
    }
    /// <summary>
    ///  Initiates an OAuth2 flow for a user to "sign in with Discord". This flow is intended for
    ///  desktop and mobile devices. If you are implementing for the console, leverage the device
    ///  auth flow instead (Client::GetTokenFromDevice or Client::OpenAuthorizeDeviceScreen).
    /// </summary>
    /// <remarks>
    ///  ## Overview
    ///  If you're not familiar with OAuth2, some basic background: At a high level the goal of
    ///  OAuth2 is to allow a user to connect two applications together and share data between them.
    ///  In this case, allowing a game to access some of their Discord data. The high level flow is:
    ///  - This function, Authorize, is invoked to start the OAuth2 process, and the user is sent to
    ///  Discord
    ///  - On Discord, the user sees a prompt to authorize the connection, and that prompt explains
    ///  what data and functionality the game is requesting.
    ///  - Once the user approves the connection, they are redirected back to your application with
    ///  a secret code.
    ///  - You can then exchange that secret code to get back an access token which can be used to
    ///  authenticate with the SDK.
    ///
    ///  ## Public vs Confidential Clients
    ///  Normal OAuth2 requires a backend server to handle exchanging the "code" for a "token" (the
    ///  last step mentioned above). Not all games have backend servers or their own identity system
    ///  though, and for early testing of the SDK that can take some time to setup.
    ///
    ///  If desired, you can instead change your Discord application in the developer portal (on the
    ///  OAuth2 tab), to be a "public" client. This will allow you to exchange the code for a token
    ///  without a backend server, by using the GetToken function below. You can also change this
    ///  setting back once you have a backend in place later too.
    ///
    ///  ## Overlay
    ///  To streamline the authentication process, the SDK will attempt to use the Discord overlay
    ///  if it is enabled. This will allow the user to authenticate without leaving the game,
    ///  enabling a more seamless experience.
    ///
    ///  You should check to see if the Discord overlay works with your game before shipping. It's
    ///  ok if it doesn't though, the SDK will fall back to using a browser window. Once you're
    ///  ready to ship, you can work with us to have the overlay enabled by default for your game
    ///  too.
    ///
    ///  If your game's main window is not the same process that the SDK is running in, then you
    ///  need to tell the SDK the PID of the window that the overlay should attach to. You can do
    ///  this by calling Client::SetGameWindowPid.
    ///
    ///  ## Redirects
    ///  For the Authorize function to work, you must configure a redirect url in your Discord
    ///  application in the developer portal, (it is located on the OAuth2 tab).
    ///  - For desktop applications, add `http://127.0.0.1/callback`
    ///  - For mobile applications, add `discord-APP_ID:/authorize/callback`
    ///
    ///  The SDK will then spin up a local webserver to handle the OAuth2 redirects for you as
    ///  well to streamline your integration.
    ///
    ///  ## Security
    ///  This function accepts an args object, and two of those values are important for security:
    ///  - To prevent CSRF attacks during auth, the SDK automatically attaches a state and checks it
    ///  for you when performing the authorization. You can override state if you want for your own
    ///  flow, but please be mindful to keep it a secure, random value.
    ///  - If you are using the Client::GetToken function you will need to provide a "code
    ///  challenge" or "code verifier". We'll spare you the boring details of how that works (woo…
    ///  crypto), as we've made a simple function to create these for you,
    ///  Client::CreateAuthorizationCodeVerifier. That returns a struct with two items, a
    ///  `challenge` value to pass into this function and a `verifier` value to pass into
    ///  Client::GetToken.
    ///
    ///  ## Callbacks & Code Exchange
    ///  When this flow completes, the given callback function will be invoked with a "code". That
    ///  code must be exchanged for an actual authorization token before it can be used. To start,
    ///  you can use the Client::GetToken function to perform this exchange. Longer term private
    ///  apps will want to move to the server side API for this, since that enables provisional
    ///  accounts to "upgrade" to full Discord accounts.
    ///
    ///  ## Android
    ///  You must add the appropriate intent filter to your `AndroidManifest.xml`.
    ///  `AndroidBuildPostProcessor` in the sample demonstrates how to do this automatically.
    ///
    ///  If you'd like to manage it yourself, the required entry in your `<application>` looks like
    ///  this:
    ///  ```xml
    ///  <activity android:name="com.discord.socialsdk.AuthenticationActivity"
    ///  android:exported="true">
    ///    <intent-filter>
    ///      <action android:name="android.intent.action.VIEW" />
    ///      <category android:name="android.intent.category.DEFAULT" />
    ///      <category android:name="android.intent.category.BROWSABLE" />
    ///      <data android:scheme="discord-1234567890123456789" />
    ///    </intent-filter>
    ///  </activity>
    ///  ```
    ///  Replace the numbers after `discord-` with your Application ID from the Discord developer
    ///  portal.
    ///
    ///  Android support (specifically the builtin auth flow) requires the androidx.browser library
    ///  as a dependency of your app. The sample uses Google External Dependency Manager to add this
    ///  to the Gradle build for the project, but you may use any means of your choosing to add this
    ///  dependency. We currently depend on version 1.8.0 of this library.
    ///
    ///  For more information see: https://discord.com/developers/docs/topics/oauth2
    ///
    /// </remarks>
    public void Authorize(AuthorizationArgs args,
                          AuthorizationCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationArgs* __argsFixed = &args.self)
            {
                NativeMethods.Client.AuthorizationCallback __callbackDelegate =
              NativeMethods.Client.AuthorizationCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.Authorize(
                      self,
                      __argsFixed,
                      __callbackDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }
    }
    /// <summary>
    ///  This function is used to hide the device authorization screen and is used for the case
    ///  where the user is on a limited input device, such as a console or smart TV. This function
    ///  should be used in conjunction with a backend server to handle the device authorization
    ///  flow. For a public client, you can use Client::AbortGetTokenFromDevice instead.
    /// </summary>
    public void CloseAuthorizeDeviceScreen()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CloseAuthorizeDeviceScreen(self);
            }
        }
    }
    /// <summary>
    ///  Helper function that can create a code challenge and verifier for use in the
    ///  Client::Authorize + Client::GetToken flow. This returns a struct with two items, a
    ///  `challenge` value to pass into Client::Authorize and a `verifier` value to pass into
    ///  GetToken.
    /// </summary>
    public AuthorizationCodeVerifier CreateAuthorizationCodeVerifier()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.AuthorizationCodeVerifier();
            AuthorizationCodeVerifier? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CreateAuthorizationCodeVerifier(self, &__returnValueNative);
            }
            __returnValue = new AuthorizationCodeVerifier(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Fetches basic information about the user associated with the given auth token.
    /// </summary>
    /// <remarks>
    ///  This can allow you to check if an auth token is valid or not.
    ///  This does not require the client to be connected or to have it's own authentication token,
    ///  so it can be called immediately after the client connects.
    ///
    /// </remarks>
    public void FetchCurrentUser(AuthorizationTokenType tokenType,
                                 string token,
                                 FetchCurrentUserCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            NativeMethods.Client.FetchCurrentUserCallback __callbackDelegate =
          NativeMethods.Client.FetchCurrentUserCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.FetchCurrentUser(
                  self,
                  tokenType,
                  __tokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__tokenSpan, __tokenOwned);
        }
    }
    /// <summary>
    ///  Provisional accounts are a way for users that have not signed up for Discord to still
    ///  access SDK functionality. They are "placeholder" Discord accounts for the user that are
    ///  owned and managed by your game. Provisional accounts exist so that your users can engage
    ///  with Discord APIs and systems without the friction of creating their own Discord account.
    ///  Provisional accounts and their data are unique per Discord application.
    /// </summary>
    /// <remarks>
    ///  This function generates a Discord access token. You pass in the "identity" of the user, and
    ///  it generates a new Discord account that is tied to that identity. There are multiple ways
    ///  of specifying that identity, including using Steam/Epic services, or using your own
    ///  identity system.
    ///
    ///  The callback function will be invoked with an access token that expires in 1 hour. Refresh
    ///  tokens are not supported for provisional accounts, so that will be an empty string. You
    ///  will need to call this function again to get a new access token when the old one expires.
    ///
    ///  NOTE: When the token expires the SDK will still continue to receive updates such as new
    ///  messages sent in a lobby, and any voice calls will continue to be active. But any new
    ///  actions taken will fail such as sending a messaging or adding a friend. You can get a new
    ///  token and pass it to UpdateToken without interrupting the user's experience.
    ///
    ///  It is suggested that these provisional tokens are not stored, and instead to just invoke
    ///  this function each time the game is launched and when these tokens are about to expire.
    ///  However, should you choose to store it, it is recommended to differentiate these
    ///  provisional account tokens from "full" Discord account tokens.
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void GetProvisionalToken(ulong applicationId,
                                    AuthenticationExternalAuthType externalAuthType,
                                    string externalAuthToken,
                                    TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __externalAuthTokenSpan;
            var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
            NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
          NativeMethods.Client.TokenExchangeCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetProvisionalToken(
                  self,
                  applicationId,
                  externalAuthType,
                  __externalAuthTokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__externalAuthTokenSpan, __externalAuthTokenOwned);
        }
    }
    /// <summary>
    ///  Exchanges an authorization code that was returned from the Client::Authorize function
    ///  for an access token which can be used to authenticate with the SDK.
    /// </summary>
    /// <remarks>
    ///  The callback function will be invoked with two tokens:
    ///  - An access token which can be used to authenticate with the SDK, but expires after 7 days.
    ///  - A refresh token, which cannot be used to authenticate, but can be used to get a new
    ///  access token later. Refresh tokens do not currently expire.
    ///
    ///  It will also include when the access token expires in seconds.
    ///  You will want to store this value as well and refresh the token when it gets close to
    ///  expiring (for example if the user launches the game and the token expires within 24 hours,
    ///  it would be good to refresh it).
    ///
    ///  For more information see https://discord.com/developers/docs/topics/oauth2
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void GetToken(ulong applicationId,
                         string code,
                         string codeVerifier,
                         string redirectUri,
                         TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __codeSpan;
            var __codeOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__codeSpan, code);
            NativeMethods.Discord_String __codeVerifierSpan;
            var __codeVerifierOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__codeVerifierSpan, codeVerifier);
            NativeMethods.Discord_String __redirectUriSpan;
            var __redirectUriOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__redirectUriSpan, redirectUri);
            NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
          NativeMethods.Client.TokenExchangeCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetToken(self,
                                              applicationId,
                                              __codeSpan,
                                              __codeVerifierSpan,
                                              __redirectUriSpan,
                                              __callbackDelegate,
                                              NativeMethods.ManagedUserData.Free,
                                              NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__redirectUriSpan, __redirectUriOwned);
            NativeMethods.__FreeLocal(&__codeVerifierSpan, __codeVerifierOwned);
            NativeMethods.__FreeLocal(&__codeSpan, __codeOwned);
        }
    }
    /// <summary>
    ///  This function is a combination of Client::Authorize and Client::GetToken, but is used for
    ///  the case where the user is on a limited input device, such as a console or smart TV.
    /// </summary>
    /// <remarks>
    ///  The callback function will be invoked with two tokens:
    ///  - An access token which can be used to authenticate with the SDK, but expires after 7 days.
    ///  - A refresh token, which cannot be used to authenticate, but can be used to get a new
    ///  access token later. Refresh tokens do not currently expire.
    ///
    ///  It will also include when the access token expires in seconds.
    ///  You will want to store this value as well and refresh the token when it gets close to
    ///  expiring (for example if the user launches the game and the token expires within 24 hours,
    ///  it would be good to refresh it).
    ///
    ///  For more information see https://discord.com/developers/docs/topics/oauth2
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic. If you have a backend server for auth, you can use
    ///  Client::OpenAuthorizeDeviceScreen and Client::CloseAuthorizeDeviceScreen to show/hide the
    ///  UI for the device auth flow.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void GetTokenFromDevice(DeviceAuthorizationArgs args,
                                   TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* __argsFixed = &args.self)
            {
                NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
              NativeMethods.Client.TokenExchangeCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetTokenFromDevice(
                      self,
                      __argsFixed,
                      __callbackDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(callback));
                }
            }
        }
    }
    /// <summary>
    ///  This function is a combination of Client::Authorize and
    ///  Client::GetTokenFromProvisionalMerge, but is used for the case where the user is on a
    ///  limited input device, such as a console or smart TV.
    /// </summary>
    /// <remarks>
    ///  This function should be used whenever a user with a provisional account wants to link to an
    ///  existing Discord account or "upgrade" their provisional account into a "full" Discord
    ///  account.
    ///
    ///  In this case, data from the provisional account should be "migrated" to the Discord
    ///  account, a process we call "account merging". Specifically relationships, DMs, and lobby
    ///  memberships are transferred to the Discord account.
    ///
    ///  The provisional account will be deleted once this merging process completes. If the user
    ///  later unlinks, then a new provisional account with a new unique ID is created.
    ///
    ///  The account merging process starts the same as the normal login flow, by invoking the
    ///  GetTokenFromDevice. But instead of calling GetTokenFromDevice, call this function and pass
    ///  in the provisional user's identity as well.
    ///
    ///  The Discord backend can then find both the provisional account with that identity and the
    ///  new Discord account and merge any data as necessary.
    ///
    ///  See the documentation for GetTokenFromDevice for more details on the callback. Note that
    ///  the callback will be invoked when the token exchange completes, but the process of merging
    ///  accounts happens asynchronously so will not be complete yet.
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic. If you have a backend server for auth, you can use
    ///  Client::OpenAuthorizeDeviceScreen and Client::CloseAuthorizeDeviceScreen to show/hide the
    ///  UI for the device auth flow.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void GetTokenFromDeviceProvisionalMerge(
      DeviceAuthorizationArgs args,
      AuthenticationExternalAuthType externalAuthType,
      string externalAuthToken,
      TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* __argsFixed = &args.self)
            {
                var __scratch = stackalloc byte[1024];
                var __scratchUsed = 0;
                NativeMethods.Discord_String __externalAuthTokenSpan;
                var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
                  __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
                NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
              NativeMethods.Client.TokenExchangeCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.GetTokenFromDeviceProvisionalMerge(
                      self,
                      __argsFixed,
                      externalAuthType,
                      __externalAuthTokenSpan,
                      __callbackDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(callback));
                }
                NativeMethods.__FreeLocal(&__externalAuthTokenSpan, __externalAuthTokenOwned);
            }
        }
    }
    /// <summary>
    ///  This function should be used with the Client::Authorize function whenever a user with a
    ///  provisional account wants to link to an existing Discord account or "upgrade" their
    ///  provisional account into a "full" Discord account.
    /// </summary>
    /// <remarks>
    ///  In this case, data from the provisional account should be "migrated" to the Discord
    ///  account, a process we call "account merging". Specifically relationships, DMs, and lobby
    ///  memberships are transferred to the Discord account.
    ///
    ///  The provisional account will be deleted once this merging process completes. If the user
    ///  later unlinks, then a new provisional account with a new unique ID is created.
    ///
    ///  The account merging process starts the same as the normal login flow, by invoking the
    ///  Authorize method to get an authorization code back. But instead of calling GetToken, call
    ///  this function and pass in the provisional user's identity as well.
    ///
    ///  The Discord backend can then find both the provisional account with that identity and the
    ///  new Discord account and merge any data as necessary.
    ///
    ///  See the documentation for GetToken for more details on the callback. Note that the callback
    ///  will be invoked when the token exchange completes, but the process of merging accounts
    ///  happens asynchronously so will not be complete yet.
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void GetTokenFromProvisionalMerge(
      ulong applicationId,
      string code,
      string codeVerifier,
      string redirectUri,
      AuthenticationExternalAuthType externalAuthType,
      string externalAuthToken,
      TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __codeSpan;
            var __codeOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__codeSpan, code);
            NativeMethods.Discord_String __codeVerifierSpan;
            var __codeVerifierOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__codeVerifierSpan, codeVerifier);
            NativeMethods.Discord_String __redirectUriSpan;
            var __redirectUriOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__redirectUriSpan, redirectUri);
            NativeMethods.Discord_String __externalAuthTokenSpan;
            var __externalAuthTokenOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__externalAuthTokenSpan, externalAuthToken);
            NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
          NativeMethods.Client.TokenExchangeCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetTokenFromProvisionalMerge(
                  self,
                  applicationId,
                  __codeSpan,
                  __codeVerifierSpan,
                  __redirectUriSpan,
                  externalAuthType,
                  __externalAuthTokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__externalAuthTokenSpan, __externalAuthTokenOwned);
            NativeMethods.__FreeLocal(&__redirectUriSpan, __redirectUriOwned);
            NativeMethods.__FreeLocal(&__codeVerifierSpan, __codeVerifierOwned);
            NativeMethods.__FreeLocal(&__codeSpan, __codeOwned);
        }
    }
    /// <summary>
    ///  Returns true if the SDK has a non-empty OAuth2 token set, regardless of whether that token
    ///  is valid or not.
    /// </summary>
    public bool IsAuthenticated()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.IsAuthenticated(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  This function is used to show the device authorization screen and is used for the case
    ///  where the user is on a limited input device, such as a console or smart TV. This function
    ///  should be used in conjunction with a backend server to handle the device authorization
    ///  flow. For a public client, you can use Client::GetTokenFromDevice instead.
    /// </summary>
    public void OpenAuthorizeDeviceScreen(ulong clientId, string userCode)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __userCodeSpan;
            var __userCodeOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__userCodeSpan, userCode);
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.OpenAuthorizeDeviceScreen(self, clientId, __userCodeSpan);
            }
            NativeMethods.__FreeLocal(&__userCodeSpan, __userCodeOwned);
        }
    }
    /// <summary>
    ///  Some functions don't work for provisional accounts, and require the user
    ///  merge their account into a full Discord account before proceeding. This
    ///  callback is invoked when an account merge must take place before
    ///  proceeding. The developer is responsible for initiating the account merge,
    ///  and then calling Client::ProvisionalUserMergeCompleted to signal to the SDK that
    ///  the pending operation can continue with the new account.
    /// </summary>
    public void ProvisionalUserMergeCompleted(bool success)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.ProvisionalUserMergeCompleted(self, success);
            }
        }
    }
    /// <summary>
    ///  Generates a new access token for the current user from a refresh token.
    /// </summary>
    /// <remarks>
    ///  Once this is called, the old access and refresh tokens are both invalidated and cannot be
    ///  used again. The callback function will be invoked with a new access and refresh token. See
    ///  GetToken for more details.
    ///
    ///  NOTE: This function only works for public clients. Public clients are ones that do not have
    ///  a backend server or their own concept of user accounts and simply rely on a separate system
    ///  for authentication like Steam/Epic.
    ///
    ///  When first testing the SDK, it can be a lot easier to use a public client to get a proof of
    ///  concept going, and change it to a confidential client later. You can toggle that setting on
    ///  the OAuth2 page for your application in the Discord developer portal,
    ///  https://discord.com/developers/applications
    ///
    /// </remarks>
    public void RefreshToken(ulong applicationId,
                             string refreshToken,
                             TokenExchangeCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __refreshTokenSpan;
            var __refreshTokenOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__refreshTokenSpan, refreshToken);
            NativeMethods.Client.TokenExchangeCallback __callbackDelegate =
          NativeMethods.Client.TokenExchangeCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.RefreshToken(
                  self,
                  applicationId,
                  __refreshTokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__refreshTokenSpan, __refreshTokenOwned);
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked when the device authorization screen is closed.
    /// </summary>
    public void SetAuthorizeDeviceScreenClosedCallback(
      AuthorizeDeviceScreenClosedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.AuthorizeDeviceScreenClosedCallback __cbDelegate =
          NativeMethods.Client.AuthorizeDeviceScreenClosedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetAuthorizeDeviceScreenClosedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  When users are linking their account with Discord, which involves an OAuth2 flow,
    ///  the SDK can streamline it by using Discord's overlay so the interaction happens entirely
    ///  in-game. If your game's main window is not the same process as the one running the
    ///  integration you may need to set the window PID using this method. It defaults to the
    ///  current pid.
    /// </summary>
    public void SetGameWindowPid(int pid)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetGameWindowPid(self, pid);
            }
        }
    }
    /// <summary>
    ///  Get a notification when the current token is about to expire or expired.
    /// </summary>
    /// <remarks>
    ///  This callback is invoked when the SDK detects that the last token passed to
    ///  Client::UpdateToken is nearing expiration or has expired. This is a signal to the developer
    ///  to refresh the token. The callback is invoked once per token, and will not be invoked again
    ///  until a new token is passed to Client::UpdateToken.
    ///
    ///  If the token is refreshed before the expiration callback is invoked, call
    ///  Client::UpdateToken to pass in the new token and reconfigure the token expiration.
    ///
    ///  If your client is disconnected (the token was expired when connecting or was revoked while
    ///  connected), the expiration callback will not be invoked.
    ///
    /// </remarks>
    public void SetTokenExpirationCallback(TokenExpirationCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.TokenExpirationCallback __callbackDelegate =
          NativeMethods.Client.TokenExpirationCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetTokenExpirationCallback(
                  self,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Updates the display name of a provisional account to the specified name.
    /// </summary>
    /// <remarks>
    ///  This should generally be invoked whenever the SDK starts and whenever a provisional account
    ///  changes their name, since the auto-generated name for provisional accounts is just a random
    ///  string.
    ///
    /// </remarks>
    public void UpdateProvisionalAccountDisplayName(
      string name,
      UpdateProvisionalAccountDisplayNameCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __nameSpan;
            var __nameOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__nameSpan, name);
            NativeMethods.Client
          .UpdateProvisionalAccountDisplayNameCallback __callbackDelegate =
          NativeMethods.Client.UpdateProvisionalAccountDisplayNameCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.UpdateProvisionalAccountDisplayName(
                  self,
                  __nameSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__nameSpan, __nameOwned);
        }
    }
    /// <summary>
    ///  Asynchronously sets a new auth token for this client to use.
    /// </summary>
    /// <remarks>
    ///  If your client is already connected, this function *may* trigger a reconnect.
    ///  If your client is not connected, this function will only update the auth token, and so you
    ///  must invoke Client::Connect as well. You should wait for the given callback function to be
    ///  invoked though so that the next Client::Connect attempt uses the updated token.
    ///
    /// </remarks>
    public void UpdateToken(AuthorizationTokenType tokenType,
                            string token,
                            UpdateTokenCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __tokenSpan;
            var __tokenOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__tokenSpan, token);
            NativeMethods.Client.UpdateTokenCallback __callbackDelegate =
          NativeMethods.Client.UpdateTokenCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.UpdateToken(
                  self,
                  tokenType,
                  __tokenSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__tokenSpan, __tokenOwned);
        }
    }
    /// <summary>
    ///  Returns true if the given message is able to be viewed in a Discord client.
    /// </summary>
    /// <remarks>
    ///  Not all chat messages are replicated to Discord. For example lobby chat and some DMs
    ///  are ephemeral and not persisted on Discord so cannot be opened. This function checks those
    ///  conditions and makes sure the message is viewable in Discord.
    ///
    /// </remarks>
    public bool CanOpenMessageInDiscord(ulong messageId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.CanOpenMessageInDiscord(self, messageId);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Deletes the specified message sent by the current user to the specified recipient.
    /// </summary>
    public void DeleteUserMessage(ulong recipientId,
                                  ulong messageId,
                                  DeleteUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.DeleteUserMessageCallback __cbDelegate =
          NativeMethods.Client.DeleteUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.DeleteUserMessage(
                  self,
                  recipientId,
                  messageId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Edits the specified message sent by the current user to the specified recipient.
    /// </summary>
    /// <remarks>
    ///  All of the same restrictions apply as for sending a message, see SendUserMessage for more.
    ///
    /// </remarks>
    public void EditUserMessage(ulong recipientId,
                                ulong messageId,
                                string content,
                                EditUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Client.EditUserMessageCallback __cbDelegate =
          NativeMethods.Client.EditUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.EditUserMessage(
                  self,
                  recipientId,
                  messageId,
                  __contentSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Returns a reference to the Discord channel object for the given ID.
    /// </summary>
    /// <remarks>
    ///  All messages in Discord are sent in a channel, so the most common use for this will be
    ///  to look up the channel a message was sent in.
    ///  For convience this API will also work with lobbies, so the three possible return values
    ///  for the SDK are a DM, an Ephemeral DM, and a Lobby.
    ///
    /// </remarks>
    public ChannelHandle? GetChannelHandle(ulong channelId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ChannelHandle();
            ChannelHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.GetChannelHandle(self, channelId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ChannelHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a reference to the Discord message object for the given ID.
    /// </summary>
    /// <remarks>
    ///  The SDK keeps the 25 most recent messages in each channel in memory.
    ///  Messages sent before the SDK was started cannot be accessed with this.
    ///
    /// </remarks>
    public MessageHandle? GetMessageHandle(ulong messageId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.MessageHandle();
            MessageHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.GetMessageHandle(self, messageId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new MessageHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Opens the given message in the Discord client.
    /// </summary>
    /// <remarks>
    ///  This is useful when a message is sent that contains content that cannot be viewed in
    ///  Discord. You can call this function in the click handler for any CTA you show to view the
    ///  message in Discord.
    ///
    /// </remarks>
    public void OpenMessageInDiscord(
      ulong messageId,
      ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
      OpenMessageInDiscordCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client
          .ProvisionalUserMergeRequiredCallback __provisionalUserMergeRequiredCallbackDelegate =
          NativeMethods.Client.ProvisionalUserMergeRequiredCallback_Handler;
            NativeMethods.Client.OpenMessageInDiscordCallback __callbackDelegate =
          NativeMethods.Client.OpenMessageInDiscordCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.OpenMessageInDiscord(
                  self,
                  messageId,
                  __provisionalUserMergeRequiredCallbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(provisionalUserMergeRequiredCallback),
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Sends a message in a lobby chat to all members of the lobby.
    /// </summary>
    /// <remarks>
    ///  The content of the message is restricted to 2,000 characters maximum.
    ///  See https://discord.com/developers/docs/resources/message for more details.
    ///
    ///  The content of the message can also contain special markup for formatting if desired, see
    ///  https://discord.com/developers/docs/reference#message-formatting for more details.
    ///
    ///  If the lobby is linked to a channel, the message will also be sent to that channel on
    ///  Discord.
    ///
    /// </remarks>
    public void SendLobbyMessage(ulong lobbyId,
                                 string content,
                                 SendUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Client.SendUserMessageCallback __cbDelegate =
          NativeMethods.Client.SendUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendLobbyMessage(
                  self,
                  lobbyId,
                  __contentSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Variant of Client::SendLobbyMessage that also accepts metadata to be sent with the message.
    /// </summary>
    /// <remarks>
    ///  Metadata is just simple string key/value pairs.
    ///  An example use case for this might be to include the name of the character that sent a
    ///  message.
    ///
    /// </remarks>
    public void SendLobbyMessageWithMetadata(ulong lobbyId,
                                             string content,
                                             Dictionary<string, string> metadata,
                                             SendUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Discord_Properties __metadataNative;
            __metadataNative.size = (IntPtr)metadata.Count;
            NativeMethods.Discord_String* __metadataKeys;
            NativeMethods.Discord_String* __metadataValues;
            bool* __metadataKeyOwnership;
            bool* __metadataValueOwnership;
            var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
            var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
            var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
            var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
            {
                int __i = 0;
                foreach (var (__metadataKey, __metadataValue) in metadata)
                {
                    NativeMethods.Discord_String __metadataKeySpan;
                    NativeMethods.Discord_String __metadataValueSpan;
                    __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                    __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                    __metadataKeys[__i] = __metadataKeySpan;
                    __metadataValues[__i] = __metadataValueSpan;
                    __i++;
                }
            }
            __metadataNative.keys = __metadataKeys;
            __metadataNative.values = __metadataValues;
            NativeMethods.Client.SendUserMessageCallback __cbDelegate =
          NativeMethods.Client.SendUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendLobbyMessageWithMetadata(
                  self,
                  lobbyId,
                  __contentSpan,
                  __metadataNative,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            for (int __i = 0; __i < (int)__metadataNative.size; __i++)
            {
                NativeMethods.__FreeLocal(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                NativeMethods.__FreeLocal(&__metadataValues[__i], __metadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
            NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Sends a direct message to the specified user.
    /// </summary>
    /// <remarks>
    ///  The content of the message is restricted to 2,000 characters maximum.
    ///  See https://discord.com/developers/docs/resources/message for more details.
    ///
    ///  The content of the message can also contain special markup for formatting if desired, see
    ///  https://discord.com/developers/docs/reference#message-formatting for more details.
    ///
    ///  A message can be sent between two users in the following situations:
    ///  - Both users are online and in the game and have not blocked each other
    ///  - Both users are friends with each other
    ///  - Both users share a mutual Discord server and have previously DM'd each other on Discord
    ///
    /// </remarks>
    public void SendUserMessage(ulong recipientId,
                                string content,
                                SendUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Client.SendUserMessageCallback __cbDelegate =
          NativeMethods.Client.SendUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendUserMessage(
                  self,
                  recipientId,
                  __contentSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Variant of Client::SendUserMessage that also accepts metadata to be sent with the message.
    /// </summary>
    /// <remarks>
    ///  Metadata is just simple string key/value pairs.
    ///  An example use case for this might be to include the name of the character that sent a
    ///  message.
    ///
    /// </remarks>
    public void SendUserMessageWithMetadata(ulong recipientId,
                                            string content,
                                            Dictionary<string, string> metadata,
                                            SendUserMessageCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Discord_Properties __metadataNative;
            __metadataNative.size = (IntPtr)metadata.Count;
            NativeMethods.Discord_String* __metadataKeys;
            NativeMethods.Discord_String* __metadataValues;
            bool* __metadataKeyOwnership;
            bool* __metadataValueOwnership;
            var __metadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeys, metadata.Count);
            var __metadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__metadataValues, metadata.Count);
            var __metadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataKeyOwnership, metadata.Count);
            var __metadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__metadataValueOwnership, metadata.Count);
            {
                int __i = 0;
                foreach (var (__metadataKey, __metadataValue) in metadata)
                {
                    NativeMethods.Discord_String __metadataKeySpan;
                    NativeMethods.Discord_String __metadataValueSpan;
                    __metadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataKeySpan, __metadataKey);
                    __metadataValueOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__metadataValueSpan, __metadataValue);
                    __metadataKeys[__i] = __metadataKeySpan;
                    __metadataValues[__i] = __metadataValueSpan;
                    __i++;
                }
            }
            __metadataNative.keys = __metadataKeys;
            __metadataNative.values = __metadataValues;
            NativeMethods.Client.SendUserMessageCallback __cbDelegate =
          NativeMethods.Client.SendUserMessageCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendUserMessageWithMetadata(
                  self,
                  recipientId,
                  __contentSpan,
                  __metadataNative,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            for (int __i = 0; __i < (int)__metadataNative.size; __i++)
            {
                NativeMethods.__FreeLocal(&__metadataKeys[__i], __metadataKeyOwnership[__i]);
                NativeMethods.__FreeLocal(&__metadataValues[__i], __metadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__metadataKeys, __metadataKeysOwned);
            NativeMethods.__FreeLocal(__metadataValues, __metadataValuesOwned);
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked whenever a new message is received in either a lobby or a DM.
    /// </summary>
    /// <remarks>
    ///  From the messageId you can fetch the MessageHandle and then the ChannelHandle to determine
    ///  the location the message was sent as well.
    ///
    ///  If the user has the Discord desktop application open on the same machine as the game, then
    ///  they will hear notifications from the Discord application, even though they are able to see
    ///  those messages in game. So to avoid double-notifying users, you should call
    ///  Client::SetShowingChat whenever the chat is shown or hidden to suppress those duplicate
    ///  notifications.
    ///
    /// </remarks>
    public void SetMessageCreatedCallback(MessageCreatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.MessageCreatedCallback __cbDelegate =
          NativeMethods.Client.MessageCreatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetMessageCreatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked whenever a message is deleted.
    /// </summary>
    /// <remarks>
    ///  Some messages sent from in game, as well as all messages sent from a connected user's
    ///  Discord client can be edited and deleted in the Discord client. So it is valuable to
    ///  implement support for this callback so that if a user edits or deletes a message in the
    ///  Discord client, it is reflected in game as well.
    ///
    /// </remarks>
    public void SetMessageDeletedCallback(MessageDeletedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.MessageDeletedCallback __cbDelegate =
          NativeMethods.Client.MessageDeletedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetMessageDeletedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked whenever a message is edited.
    /// </summary>
    /// <remarks>
    ///  Some messages sent from in game, as well as all messages sent from a connected user's
    ///  Discord client can be edited and deleted in the Discord client. So it is valuable to
    ///  implement support for this callback so that if a user edits or deletes a message in the
    ///  Discord client, it is reflected in game as well.
    ///
    /// </remarks>
    public void SetMessageUpdatedCallback(MessageUpdatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.MessageUpdatedCallback __cbDelegate =
          NativeMethods.Client.MessageUpdatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetMessageUpdatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets whether chat messages are currently being shown in the game.
    /// </summary>
    /// <remarks>
    ///  If the user has the Discord desktop application open on the same machine as the game, then
    ///  they will hear notifications from the Discord application, even though they are able to see
    ///  those messages in game. So to avoid double-notifying users, you can call this function
    ///  whenever the chat is shown or hidden to suppress those duplicate notifications.
    ///
    ///  Keep in mind, if the game stops showing chat for a period of time, or the game loses focus
    ///  because the user switches to a different app, it is important to call this function again
    ///  so that the user's notifications get re-enabled in Discord during this time.
    ///
    /// </remarks>
    public void SetShowingChat(bool showingChat)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetShowingChat(self, showingChat);
            }
        }
    }
    /// <summary>
    ///  Adds a callback function to be invoked for each new log message generated by the SDK.
    /// </summary>
    /// <remarks>
    ///  This function explicitly excludes most logs for voice and webrtc activity since those are
    ///  generally much noisier and you may want to pick a different log level for those. So it will
    ///  instead include logs for things such as lobbies, relationships, presence, and
    ///  authentication.
    ///
    ///  We strongly recommend invoking this function immediately after constructing the Client
    ///  object.
    ///
    /// </remarks>
    public void AddLogCallback(LogCallback callback,
                               LoggingSeverity minSeverity)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LogCallback __callbackDelegate =
          NativeMethods.Client.LogCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AddLogCallback(
                  self,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback),
                  minSeverity);
            }
        }
    }
    /// <summary>
    ///  Adds a callback function to be invoked for each new log message generated by the voice
    ///  subsystem of the SDK, including the underlying webrtc infrastructure.
    /// </summary>
    /// <remarks>
    ///  We strongly recommend invoking this function immediately after constructing the Client
    ///  object.
    ///
    /// </remarks>
    public void AddVoiceLogCallback(LogCallback callback,
                                    LoggingSeverity minSeverity)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LogCallback __callbackDelegate =
          NativeMethods.Client.LogCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AddVoiceLogCallback(
                  self,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback),
                  minSeverity);
            }
        }
    }
    /// <summary>
    ///  Asynchronously connects the client to Discord.
    /// </summary>
    /// <remarks>
    ///  If a client is disconnecting, this will wait for the disconnect before reconnecting.
    ///  You should use the Client::SetStatusChangedCallback and Client::GetStatus functions to
    ///  receive updates on the client status. The Client is only safe to use once the status
    ///  changes to Status::Ready.
    ///
    /// </remarks>
    public void Connect()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.Connect(self);
            }
        }
    }
    /// <summary>
    ///  Asynchronously disconnects the client.
    /// </summary>
    /// <remarks>
    ///  You can leverage Client::SetStatusChangedCallback and Client::GetStatus to receive updates
    ///  on the client status. It is fully disconnected when the status changes to
    ///  Client::Status::Disconnected.
    ///
    /// </remarks>
    public void Disconnect()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.Disconnect(self);
            }
        }
    }
    /// <summary>
    ///  Returns the current status of the client, see the Status enum for an explanation of the
    ///  possible values.
    /// </summary>
    public Status GetStatus()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            Status __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.GetStatus(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  This function is used to set the application ID for the client. This is used to
    ///  identify the application to the Discord client. This is used for things like
    ///  authentication, rich presence, and activity invites when *not* connected with
    ///  Client::Connect. When calling Client::Connect, the application ID is set automatically
    /// </summary>
    public void SetApplicationId(ulong applicationId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetApplicationId(self, applicationId);
            }
        }
    }
    /// <summary>
    ///  Causes logs generated by the SDK to be written to disk in the specified directory.
    /// </summary>
    /// <remarks>
    ///  This function explicitly excludes most logs for voice and webrtc activity since those are
    ///  generally much noisier and you may want to pick a different log level for those. So it will
    ///  instead include logs for things such as lobbies, relationships, presence, and
    ///  authentication. An empty path defaults to logging alongside the client library. A
    ///  minSeverity = LoggingSeverity::None disables logging to a file (also the current default).
    ///  The logs will be placed into a file called "discord.log" in the specified directory.
    ///  Overwrites any existing discord.log file.
    ///
    ///  We strongly recommend invoking this function immediately after constructing the Client
    ///  object.
    ///
    ///  Returns true if the log file was successfully opened, false otherwise.
    ///
    /// </remarks>
    public bool SetLogDir(string path, LoggingSeverity minSeverity)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __pathSpan;
            var __pathOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__pathSpan, path);
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.SetLogDir(self, __pathSpan, minSeverity);
            }
            NativeMethods.__FreeLocal(&__pathSpan, __pathOwned);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever the SDKs status changes.
    /// </summary>
    public void SetStatusChangedCallback(OnStatusChanged cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.OnStatusChanged __cbDelegate =
          NativeMethods.Client.OnStatusChanged_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetStatusChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Causes logs generated by the voice subsystem of the SDK to be written to disk in the
    ///  specified directory.
    /// </summary>
    /// <remarks>
    ///  These logs will be in a file like discord-webrtc_0, and if they grow to big will be rotated
    ///  and the number incremented. If the log files already exist the old ones will be renamed to
    ///  discord-last-webrtc_0.
    ///
    ///  An empty path defaults to logging alongside the client library.
    ///  A minSeverity = LoggingSeverity::None disables logging to a file (also the current
    ///  default).
    ///
    ///  WARNING: This function MUST be invoked immediately after constructing the Client object!
    ///  It will print out a warning if invoked too late.
    ///
    /// </remarks>
    public void SetVoiceLogDir(string path, LoggingSeverity minSeverity)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __pathSpan;
            var __pathOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__pathSpan, path);
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetVoiceLogDir(self, __pathSpan, minSeverity);
            }
            NativeMethods.__FreeLocal(&__pathSpan, __pathOwned);
        }
    }
    /// <summary>
    ///  Joins the user to the specified lobby, creating one if it does not exist.
    /// </summary>
    /// <remarks>
    ///  The lobby is specified by the supplied string, which should be a hard to guess secret
    ///  generated by the game. All users who join the lobby with the same secret will be placed in
    ///  the same lobby.
    ///
    ///  For exchanging the secret, we strongly encourage looking into the activity invite and rich
    ///  presence systems which provide a way to include a secret string that only accepted party
    ///  members are able to see.
    ///
    ///  As with server created lobbies, client created lobbies auto-delete once they have been idle
    ///  for a few minutes (which currently defaults to 5 minutes). A lobby is idle if no users are
    ///  connected to it.
    ///
    ///  This function shouldn't be used for long lived lobbies. The "secret" value expires after
    ///  ~30 days, at which point the existing lobby cannot be joined and a new one would be created
    ///  instead.
    ///
    /// </remarks>
    public void CreateOrJoinLobby(string secret,
                                  CreateOrJoinLobbyCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __secretSpan;
            var __secretOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__secretSpan, secret);
            NativeMethods.Client.CreateOrJoinLobbyCallback __callbackDelegate =
          NativeMethods.Client.CreateOrJoinLobbyCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CreateOrJoinLobby(
                  self,
                  __secretSpan,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            NativeMethods.__FreeLocal(&__secretSpan, __secretOwned);
        }
    }
    /// <summary>
    ///  Variant of Client::CreateOrJoinLobby that also accepts developer-supplied metadata.
    /// </summary>
    /// <remarks>
    ///  Metadata is just simple string key/value pairs.
    ///  An example use case for this might be to the internal game ID of the user of each lobby so
    ///  all members of the lobby can have a mapping of discord IDs to game IDs. Subsequent calls to
    ///  CreateOrJoinLobby will overwrite the metadata for the lobby and member.
    ///
    /// </remarks>
    public void CreateOrJoinLobbyWithMetadata(
      string secret,
      Dictionary<string, string> lobbyMetadata,
      Dictionary<string, string> memberMetadata,
      CreateOrJoinLobbyCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __secretSpan;
            var __secretOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__secretSpan, secret);
            NativeMethods.Discord_Properties __lobbyMetadataNative;
            __lobbyMetadataNative.size = (IntPtr)lobbyMetadata.Count;
            NativeMethods.Discord_String* __lobbyMetadataKeys;
            NativeMethods.Discord_String* __lobbyMetadataValues;
            bool* __lobbyMetadataKeyOwnership;
            bool* __lobbyMetadataValueOwnership;
            var __lobbyMetadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeys, lobbyMetadata.Count);
            var __lobbyMetadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataValues, lobbyMetadata.Count);
            var __lobbyMetadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeyOwnership, lobbyMetadata.Count);
            var __lobbyMetadataValueOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__lobbyMetadataValueOwnership, lobbyMetadata.Count);
            {
                int __i = 0;
                foreach (var (__lobbyMetadataKey, __lobbyMetadataValue) in lobbyMetadata)
                {
                    NativeMethods.Discord_String __lobbyMetadataKeySpan;
                    NativeMethods.Discord_String __lobbyMetadataValueSpan;
                    __lobbyMetadataKeyOwnership[__i] = NativeMethods.__InitStringLocal(
                      __scratch, &__scratchUsed, 1024, &__lobbyMetadataKeySpan, __lobbyMetadataKey);
                    __lobbyMetadataValueOwnership[__i] =
                      NativeMethods.__InitStringLocal(__scratch,
                                                      &__scratchUsed,
                                                      1024,
                                                      &__lobbyMetadataValueSpan,
                                                      __lobbyMetadataValue);
                    __lobbyMetadataKeys[__i] = __lobbyMetadataKeySpan;
                    __lobbyMetadataValues[__i] = __lobbyMetadataValueSpan;
                    __i++;
                }
            }
            __lobbyMetadataNative.keys = __lobbyMetadataKeys;
            __lobbyMetadataNative.values = __lobbyMetadataValues;
            NativeMethods.Discord_Properties __memberMetadataNative;
            __memberMetadataNative.size = (IntPtr)memberMetadata.Count;
            NativeMethods.Discord_String* __memberMetadataKeys;
            NativeMethods.Discord_String* __memberMetadataValues;
            bool* __memberMetadataKeyOwnership;
            bool* __memberMetadataValueOwnership;
            var __memberMetadataKeysOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__memberMetadataKeys, memberMetadata.Count);
            var __memberMetadataValuesOwned = NativeMethods.__AllocLocalStringArray(
              __scratch, &__scratchUsed, 1024, &__memberMetadataValues, memberMetadata.Count);
            var __memberMetadataKeyOwnershipOwned = NativeMethods.__AllocateLocalBoolArray(
              __scratch, &__scratchUsed, 1024, &__memberMetadataKeyOwnership, memberMetadata.Count);
            var __memberMetadataValueOwnershipOwned =
              NativeMethods.__AllocateLocalBoolArray(__scratch,
                                                     &__scratchUsed,
                                                     1024,
                                                     &__memberMetadataValueOwnership,
                                                     memberMetadata.Count);
            {
                int __i = 0;
                foreach (var (__memberMetadataKey, __memberMetadataValue) in memberMetadata)
                {
                    NativeMethods.Discord_String __memberMetadataKeySpan;
                    NativeMethods.Discord_String __memberMetadataValueSpan;
                    __memberMetadataKeyOwnership[__i] =
                      NativeMethods.__InitStringLocal(__scratch,
                                                      &__scratchUsed,
                                                      1024,
                                                      &__memberMetadataKeySpan,
                                                      __memberMetadataKey);
                    __memberMetadataValueOwnership[__i] =
                      NativeMethods.__InitStringLocal(__scratch,
                                                      &__scratchUsed,
                                                      1024,
                                                      &__memberMetadataValueSpan,
                                                      __memberMetadataValue);
                    __memberMetadataKeys[__i] = __memberMetadataKeySpan;
                    __memberMetadataValues[__i] = __memberMetadataValueSpan;
                    __i++;
                }
            }
            __memberMetadataNative.keys = __memberMetadataKeys;
            __memberMetadataNative.values = __memberMetadataValues;
            NativeMethods.Client.CreateOrJoinLobbyCallback __callbackDelegate =
          NativeMethods.Client.CreateOrJoinLobbyCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CreateOrJoinLobbyWithMetadata(
                  self,
                  __secretSpan,
                  __lobbyMetadataNative,
                  __memberMetadataNative,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
            for (int __i = 0; __i < (int)__memberMetadataNative.size; __i++)
            {
                NativeMethods.__FreeLocal(&__memberMetadataKeys[__i],
                                          __memberMetadataKeyOwnership[__i]);
                NativeMethods.__FreeLocal(&__memberMetadataValues[__i],
                                          __memberMetadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__memberMetadataKeys, __memberMetadataKeysOwned);
            NativeMethods.__FreeLocal(__memberMetadataValues, __memberMetadataValuesOwned);
            for (int __i = 0; __i < (int)__lobbyMetadataNative.size; __i++)
            {
                NativeMethods.__FreeLocal(&__lobbyMetadataKeys[__i],
                                          __lobbyMetadataKeyOwnership[__i]);
                NativeMethods.__FreeLocal(&__lobbyMetadataValues[__i],
                                          __lobbyMetadataValueOwnership[__i]);
            }
            NativeMethods.__FreeLocal(__lobbyMetadataKeys, __lobbyMetadataKeysOwned);
            NativeMethods.__FreeLocal(__lobbyMetadataValues, __lobbyMetadataValuesOwned);
            NativeMethods.__FreeLocal(&__secretSpan, __secretOwned);
        }
    }
    /// <summary>
    ///  Fetches all of the channels that the current user can access in the given guild.
    /// </summary>
    /// <remarks>
    ///  The purpose of this is to power the channel linking flow for linking a Discord channel to
    ///  an in-game lobby. So this function can be used to power a UI to let the user pick which
    ///  channel to link to once they have picked a guild. See the docs on LobbyHandle for more
    ///  information.
    ///
    /// </remarks>
    public void GetGuildChannels(ulong guildId, GetGuildChannelsCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetGuildChannelsCallback __cbDelegate =
          NativeMethods.Client.GetGuildChannelsCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetGuildChannels(
                  self,
                  guildId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Returns a reference to the Discord lobby object for the given ID.
    /// </summary>
    public LobbyHandle? GetLobbyHandle(ulong lobbyId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.LobbyHandle();
            LobbyHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.GetLobbyHandle(self, lobbyId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new LobbyHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a list of all the lobbies that the user is a member of and the SDK has loaded.
    /// </summary>
    /// <remarks>
    ///  Lobbies are optimistically loaded when the SDK starts but in some cases may not be
    ///  available immediately after the SDK status changes to Status::Ready.
    ///
    /// </remarks>
    public ulong[] GetLobbyIds()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetLobbyIds(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Fetches all of the guilds (also known as Discord servers) that the current user is a member
    ///  of.
    /// </summary>
    /// <remarks>
    ///  The purpose of this is to power the channel linking flow for linking a Discord channel
    ///  to an in-game lobby. So this function can be used to power a UI to let the user which guild
    ///  to link to. See the docs on LobbyHandle for more information.
    ///
    /// </remarks>
    public void GetUserGuilds(GetUserGuildsCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.GetUserGuildsCallback __cbDelegate =
          NativeMethods.Client.GetUserGuildsCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetUserGuilds(self,
                                                   __cbDelegate,
                                                   NativeMethods.ManagedUserData.Free,
                                                   NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Removes the current user from the specified lobby.
    /// </summary>
    /// <remarks>
    ///  Only lobbies that contain a "secret" can be left.
    ///  In other words, only lobbies created with Client::CreateOrJoinLobby can be left.
    ///  Lobbies created using the server API may not be manipulated by clients, so you must
    ///  use the server API to remove them too.
    ///
    /// </remarks>
    public void LeaveLobby(ulong lobbyId, LeaveLobbyCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LeaveLobbyCallback __callbackDelegate =
          NativeMethods.Client.LeaveLobbyCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.LeaveLobby(
                  self,
                  lobbyId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Links the specified channel on Discord to the specified in-game lobby.
    /// </summary>
    /// <remarks>
    ///  Any message sent in one will be copied over to the other!
    ///  See the docs on LobbyHandle for more information.
    ///
    /// </remarks>
    public void LinkChannelToLobby(ulong lobbyId,
                                   ulong channelId,
                                   LinkOrUnlinkChannelCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LinkOrUnlinkChannelCallback __callbackDelegate =
          NativeMethods.Client.LinkOrUnlinkChannelCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.LinkChannelToLobby(
                  self,
                  lobbyId,
                  channelId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked when a lobby "becomes available" to the client.
    /// </summary>
    /// <remarks>
    ///  A lobby can become available in a few situations:
    ///  - A new lobby is created and the current user is a member of it
    ///  - The current user is added to an existing lobby
    ///  - A lobby recovers after a backend crash and is available once again
    ///
    ///  This means that the LobbyCreated callback can be invoked more than once in a single
    ///  session! Generally though it should never be invoked twice in a row. For example if a lobby
    ///  crashes or a user is removed from the lobby, you should expect to have the LobbyDeleted
    ///  callback invoked first.
    ///
    /// </remarks>
    public void SetLobbyCreatedCallback(LobbyCreatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyCreatedCallback __cbDelegate =
          NativeMethods.Client.LobbyCreatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyCreatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked when a lobby is no longer available.
    /// </summary>
    /// <remarks>
    ///  This callback can be invoked in a number of situations:
    ///  - A lobby the user is a member of is deleted
    ///  - The current user is removed from a lobby
    ///  - There is a backend crash that causes the lobby to be unavailable for all users
    ///
    ///  This means that this callback might be invoked even though the lobby still exists for other
    ///  users!
    ///
    /// </remarks>
    public void SetLobbyDeletedCallback(LobbyDeletedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyDeletedCallback __cbDelegate =
          NativeMethods.Client.LobbyDeletedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyDeletedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever a user is added to a lobby.
    /// </summary>
    /// <remarks>
    ///  This callback will not be invoked when the current user is added to a lobby, instead the
    ///  LobbyCreated callback will be invoked. Additionally, the SDK separates membership in a
    ///  lobby from whether a user is connected to a lobby. So a user being added does not
    ///  necessarily mean they are online and in the lobby at that moment, just that they have
    ///  permission to connect to that lobby.
    ///
    /// </remarks>
    public void SetLobbyMemberAddedCallback(LobbyMemberAddedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyMemberAddedCallback __cbDelegate =
          NativeMethods.Client.LobbyMemberAddedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyMemberAddedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever a member of a lobby is removed and can no
    ///  longer connect to it.
    /// </summary>
    /// <remarks>
    ///  This callback will not be invoked when the current user is removed from a lobby, instead
    ///  LobbyDeleted callback will be invoked. Additionally this is not invoked when a user simply
    ///  exits the game. That would cause the LobbyMemberUpdatedCallback to be invoked, and the
    ///  LobbyMemberHandle object will indicate they are not connected now.
    ///
    /// </remarks>
    public void SetLobbyMemberRemovedCallback(LobbyMemberRemovedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyMemberRemovedCallback __cbDelegate =
          NativeMethods.Client.LobbyMemberRemovedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyMemberRemovedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever a member of a lobby is changed.
    /// </summary>
    /// <remarks>
    ///  This is invoked when:
    ///  - The user connects or disconnects
    ///  - The metadata of the member is changed
    ///
    /// </remarks>
    public void SetLobbyMemberUpdatedCallback(LobbyMemberUpdatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyMemberUpdatedCallback __cbDelegate =
          NativeMethods.Client.LobbyMemberUpdatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyMemberUpdatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked when a lobby is edited, for example if the lobby's metadata
    ///  is changed.
    /// </summary>
    public void SetLobbyUpdatedCallback(LobbyUpdatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LobbyUpdatedCallback __cbDelegate =
          NativeMethods.Client.LobbyUpdatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetLobbyUpdatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Removes any existing channel link from the specified lobby.
    /// </summary>
    /// <remarks>
    ///  See the docs on LobbyHandle for more information.
    ///  A lobby can be unlinked by any user with the LobbyMemberFlags::CanLinkLobby flag, they do
    ///  not need to have any permissions on the Discord channel in order to sever the in-game link.
    ///
    /// </remarks>
    public void UnlinkChannelFromLobby(ulong lobbyId,
                                       LinkOrUnlinkChannelCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.LinkOrUnlinkChannelCallback __callbackDelegate =
          NativeMethods.Client.LinkOrUnlinkChannelCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.UnlinkChannelFromLobby(
                  self,
                  lobbyId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Accepts an activity invite that the current user has received.
    /// </summary>
    /// <remarks>
    ///  The given callback will be invoked with the join secret for the activity, which can be used
    ///  to join the user to the game's internal party system for example.
    ///  This join secret comes from the other user's rich presence activity.
    ///
    /// </remarks>
    public void AcceptActivityInvite(ActivityInvite invite,
                                     AcceptActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* __inviteFixed = &invite.self)
            {
                NativeMethods.Client.AcceptActivityInviteCallback __cbDelegate =
              NativeMethods.Client.AcceptActivityInviteCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.AcceptActivityInvite(
                      self,
                      __inviteFixed,
                      __cbDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }
    }
    /// <summary>
    ///  Clears the right presence for the current user.
    /// </summary>
    public void ClearRichPresence()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.ClearRichPresence(self);
            }
        }
    }
    /// <summary>
    ///  When a user accepts an activity invite for your game within the Discord client, Discord
    ///  needs to know how to launch the game for that user. This function allows you to register a
    ///  command that Discord will run to launch your game. You should invoke this when the SDK
    ///  starts up so that if the user in the future tries to join from Discord the game will be
    ///  able to be launched for them. Returns true if the command was successfully registered,
    ///  false otherwise.
    /// </summary>
    /// <remarks>
    ///  On Windows and Linux, this command should be a path to an executable. It also supports any
    ///  launch parameters that may be needed, like
    ///  "C:\path\to my\game.exe" --full-screen --no-hax
    ///  If you pass an empty string in for the command, the SDK will register the current running
    ///  executable. To launch the game from a custom protocol like my-awesome-game://, pass that in
    ///  as an argument of the executable that should be launched by that protocol. For example,
    ///  "C:\path\to my\game.exe" my-awesome-game://
    ///
    ///  On macOS, due to the way Discord registers executables, your game needs to be bundled for
    ///  this command to work. That means it should be a .app. You can pass a custom protocol like
    ///  my-awesome-game:// as the custom command, but *not* a path to an executable. If you pass an
    ///  empty string in for the command, the SDK will register the current running bundle, if any.
    ///
    /// </remarks>
    public bool RegisterLaunchCommand(ulong applicationId, string command)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __commandSpan;
            var __commandOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__commandSpan, command);
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue =
                  NativeMethods.Client.RegisterLaunchCommand(self, applicationId, __commandSpan);
            }
            NativeMethods.__FreeLocal(&__commandSpan, __commandOwned);
            return __returnValue;
        }
    }
    /// <summary>
    ///  When a user accepts an activity invite for your game within the Discord client, Discord
    ///  needs to know how to launch the game for that user. For steam games, this function allows
    ///  you to indicate to Discord what the steam game ID is. You should invoke this when the SDK
    ///  starts up so that if the user in the future tries to join from Discord the game will be
    ///  able to be launched for them. Returns true if the command was successfully registered,
    ///  false otherwise.
    /// </summary>
    public bool RegisterLaunchSteamApplication(ulong applicationId, uint steamAppId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnValue = NativeMethods.Client.RegisterLaunchSteamApplication(
                  self, applicationId, steamAppId);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Sends a Discord activity invite to the specified user.
    /// </summary>
    /// <remarks>
    ///  The invite is sent as a message on Discord, which means it can be sent in the following
    ///  situations:
    ///  - Both users are online and in the game and have not blocked each other
    ///  - Both users are friends with each other
    ///  - Both users share a mutual Discord server and have previously DM'd each other on Discord
    ///
    ///  You can optionally include some message content to include in the message containing the
    ///  invite, but it's ok to pass an empty string too.
    ///
    /// </remarks>
    public void SendActivityInvite(ulong userId,
                                   string content,
                                   SendActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __contentSpan;
            var __contentOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__contentSpan, content);
            NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
          NativeMethods.Client.SendActivityInviteCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendActivityInvite(
                  self,
                  userId,
                  __contentSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__contentSpan, __contentOwned);
        }
    }
    /// <summary>
    ///  Requests to join the activity of the specified user.
    /// </summary>
    /// <remarks>
    ///  This can be called whenever the target user has a rich presence activity for the current
    ///  game and that activity has space for another user to join them.
    ///
    ///  That user will basically receive an activity invite which they can accept or reject.
    ///
    /// </remarks>
    public void SendActivityJoinRequest(ulong userId,
                                        SendActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
          NativeMethods.Client.SendActivityInviteCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendActivityJoinRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  When another user requests to join the current user's party, this function is called to
    ///  to allow that user to join. Specifically this will send the original user an activity
    ///  invite which they then need to accept again.
    /// </summary>
    public void SendActivityJoinRequestReply(ActivityInvite invite,
                                             SendActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* __inviteFixed = &invite.self)
            {
                NativeMethods.Client.SendActivityInviteCallback __cbDelegate =
              NativeMethods.Client.SendActivityInviteCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.SendActivityJoinRequestReply(
                      self,
                      __inviteFixed,
                      __cbDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }
    }
    /// <summary>
    ///  Sets a callback function that is invoked when the current user receives an activity invite
    ///  from another user.
    /// </summary>
    /// <remarks>
    ///  These invites are always sent as messages, so the SDK is parsing these
    ///  messages to look for invites and invokes this callback instead. The message create callback
    ///  will not be invoked for these messages. The invite object contains all the necessary
    ///  information to identity the invite, which you can later pass to
    ///  Client::AcceptActivityInvite.
    ///
    /// </remarks>
    public void SetActivityInviteCreatedCallback(ActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.ActivityInviteCallback __cbDelegate =
          NativeMethods.Client.ActivityInviteCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetActivityInviteCreatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function that is invoked when an existing activity invite changes.
    ///  Currently, the only thing that changes on an activity invite is its validity. If the sender
    ///  goes offline or exits the party the receiver was invited to, the invite is no longer
    ///  joinable. It is possible for an invalid invite to go from invalid to valid if the sender
    ///  rejoins the activity.
    /// </summary>
    public void SetActivityInviteUpdatedCallback(ActivityInviteCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.ActivityInviteCallback __cbDelegate =
          NativeMethods.Client.ActivityInviteCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetActivityInviteUpdatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function that is invoked when the current user also has Discord running on
    ///  their computer and they accept an activity invite in the Discord client.
    /// </summary>
    /// <remarks>
    ///  This callback is invoked with the join secret from the activity rich presence, which you
    ///  can use to join them to the game's internal party system. See Activity for more information
    ///  on invites.
    ///
    /// </remarks>
    public void SetActivityJoinCallback(ActivityJoinCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.ActivityJoinCallback __cbDelegate =
          NativeMethods.Client.ActivityJoinCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetActivityJoinCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets whether a user is online/invisible/idle/dnd on Discord.
    /// </summary>
    public void SetOnlineStatus(StatusType status,
                                UpdateStatusCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateStatusCallback __callbackDelegate =
          NativeMethods.Client.UpdateStatusCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetOnlineStatus(
                  self,
                  status,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Updates the rich presence for the current user.
    /// </summary>
    /// <remarks>
    ///  You should use rich presence so that other users on Discord know this user is playing a
    ///  game and you can include some hints of what they are playing such as a character name or
    ///  map name. Rich presence also enables Discord game invites to work too!
    ///
    ///  Note: On Desktop, rich presence can be set before calling Client::Connect, but it will be
    ///  cleared if the Client connects. When Client is not connected, this sets the rich presence
    ///  in the current user's Discord client when available.
    ///
    ///  See the docs on the Activity struct for more details.
    ///
    ///  Note: The Activity object here is a partial object, fields such as name, and applicationId
    ///  cannot be set and will be overwritten by the SDK. See
    ///  https://discord.com/developers/docs/rich-presence/using-with-the-game-sdk#partial-activity-struct
    ///  for more information.
    ///
    /// </remarks>
    public void UpdateRichPresence(Activity activity,
                                   UpdateRichPresenceCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            fixed (NativeMethods.Activity* __activityFixed = &activity.self)
            {
                NativeMethods.Client.UpdateRichPresenceCallback __cbDelegate =
              NativeMethods.Client.UpdateRichPresenceCallback_Handler;
                fixed (NativeMethods.Client* self = &this.self)
                {
                    NativeMethods.Client.UpdateRichPresence(
                      self,
                      __activityFixed,
                      __cbDelegate,
                      NativeMethods.ManagedUserData.Free,
                      NativeMethods.ManagedUserData.CreateHandle(cb));
                }
            }
        }
    }
    /// <summary>
    ///  Accepts an incoming Discord friend request from the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user has not sent a Discord friend request to the current user, meaning
    ///  that the Discord relationship type between the users must be
    ///  RelationshipType::PendingIncoming.
    ///
    /// </remarks>
    public void AcceptDiscordFriendRequest(ulong userId,
                                           UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AcceptDiscordFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Accepts an incoming game friend request from the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user has not sent a game friend request to the current user, meaning
    ///  that the game relationship type between the users must be
    ///  RelationshipType::PendingIncoming.
    ///
    /// </remarks>
    public void AcceptGameFriendRequest(ulong userId,
                                        UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.AcceptGameFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Blocks the target user so that they cannot send the user friend or activity invites and
    ///  cannot message them anymore.
    /// </summary>
    /// <remarks>
    ///  Blocking a user will also remove any existing relationship
    ///  between the two users, and persists across games, so blocking a user in one game or on
    ///  Discord will block them in all other games and on Discord as well.
    ///
    /// </remarks>
    public void BlockUser(ulong userId, UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.BlockUser(self,
                                               userId,
                                               __cbDelegate,
                                               NativeMethods.ManagedUserData.Free,
                                               NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Cancels an outgoing Discord friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if a Discord friend request has not been sent to the target user, meaning
    ///  that the Discord relationship type between the users must be
    ///  RelationshipType::PendingOutgoing.
    ///
    /// </remarks>
    public void CancelDiscordFriendRequest(ulong userId,
                                           UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CancelDiscordFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Cancels an outgoing game friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if a game friend request has not been sent to the target user, meaning
    ///  that the game relationship type between the users must be
    ///  RelationshipType::PendingOutgoing.
    ///
    /// </remarks>
    public void CancelGameFriendRequest(ulong userId,
                                        UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.CancelGameFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Returns the RelationshipHandle that corresponds to the relationship between the current
    ///  user and the given user.
    /// </summary>
    public RelationshipHandle GetRelationshipHandle(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.RelationshipHandle();
            RelationshipHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetRelationshipHandle(self, userId, &__returnValueNative);
            }
            __returnValue = new RelationshipHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a list of all of the relationships the current user has with others, including all
    ///  Discord relationships and all Game relationships for the current game.
    /// </summary>
    public RelationshipHandle[] GetRelationships()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_RelationshipHandleSpan();
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetRelationships(self, &__returnValue);
            }
            var __returnValueSurface = new RelationshipHandle[(int)__returnValue.size];
            for (int __i = 0; __i < (int)__returnValue.size; __i++)
            {
                __returnValueSurface[__i] = new RelationshipHandle(__returnValue.ptr[__i], 0);
            }
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Declines an incoming Discord friend request from the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user has not sent a Discord friend request to the current user, meaning
    ///  that the Discord relationship type between the users must be
    ///  RelationshipType::PendingIncoming.
    ///
    /// </remarks>
    public void RejectDiscordFriendRequest(ulong userId,
                                           UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.RejectDiscordFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Declines an incoming game friend request from the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user has not sent a game friend request to the current user, meaning
    ///  that the game relationship type between the users must be
    ///  RelationshipType::PendingIncoming.
    ///
    /// </remarks>
    public void RejectGameFriendRequest(ulong userId,
                                        UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.RejectGameFriendRequest(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Removes any friendship between the current user and the target user. This function will
    ///  remove BOTH any Discord friendship and any game friendship between the users.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user is not currently a Discord OR game friend with the current user.
    ///
    /// </remarks>
    public void RemoveDiscordAndGameFriend(ulong userId,
                                           UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.RemoveDiscordAndGameFriend(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Removes any game friendship between the current user and the target user.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user is not currently a game friend with the current user.
    ///
    /// </remarks>
    public void RemoveGameFriend(ulong userId, UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.RemoveGameFriend(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Searches all of your friends by both username and display name, returning
    ///  a list of all friends that match the search string.
    /// </summary>
    /// <remarks>
    ///  Under the hood uses the Levenshtein distance algorithm.
    ///
    /// </remarks>
    public UserHandle[] SearchFriendsByUsername(string searchStr)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UserHandleSpan();
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __searchStrSpan;
            var __searchStrOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__searchStrSpan, searchStr);
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SearchFriendsByUsername(self, __searchStrSpan, &__returnValue);
            }
            NativeMethods.__FreeLocal(&__searchStrSpan, __searchStrOwned);
            var __returnValueSurface = new UserHandle[(int)__returnValue.size];
            for (int __i = 0; __i < (int)__returnValue.size; __i++)
            {
                __returnValueSurface[__i] = new UserHandle(__returnValue.ptr[__i], 0);
            }
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Sends a Discord friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  The target user is identified by their Discord unique username (not their DisplayName).
    ///
    ///  After the friend request is sent, each user will have a new Discord relationship created.
    ///  For the current user the RelationshipHandle::DiscordRelationshipType will be
    ///  RelationshipType::PendingOutgoing and for the target user it will be
    ///  RelationshipType::PendingIncoming.
    ///
    ///  If the current user already has received a Discord friend request from the target user
    ///  (meaning RelationshipHandle::DiscordRelationshipType is RelationshipType::PendingIncoming),
    ///  then the two users will become Discord friends.
    ///
    ///  See RelationshipHandle for more information on the difference between Discord and Game
    ///  relationships.
    ///
    /// </remarks>
    public void SendDiscordFriendRequest(string username,
                                         SendFriendRequestCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __usernameSpan;
            var __usernameOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__usernameSpan, username);
            NativeMethods.Client.SendFriendRequestCallback __cbDelegate =
          NativeMethods.Client.SendFriendRequestCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendDiscordFriendRequest(
                  self,
                  __usernameSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__usernameSpan, __usernameOwned);
        }
    }
    /// <summary>
    ///  Sends a Discord friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  The target user is identified by their Discord ID.
    ///
    ///  After the friend request is sent, each user will have a new Discord relationship created.
    ///  For the current user the RelationshipHandle::DiscordRelationshipType will be
    ///  RelationshipType::PendingOutgoing and for the target user it will be
    ///  RelationshipType::PendingIncoming.
    ///
    ///  If the current user already has received a Discord friend request from the target user
    ///  (meaning RelationshipHandle::DiscordRelationshipType is RelationshipType::PendingIncoming),
    ///  then the two users will become Discord friends.
    ///
    ///  See RelationshipHandle for more information on the difference between Discord and Game
    ///  relationships.
    ///
    /// </remarks>
    public void SendDiscordFriendRequestById(ulong userId,
                                             UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendDiscordFriendRequestById(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sends (or accepts) a game friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  The target user is identified by their Discord unique username (not their DisplayName).
    ///
    ///  After the friend request is sent, each user will have a new game relationship created. For
    ///  the current user the RelationshipHandle::GameRelationshipType will be
    ///  RelationshipType::PendingOutgoing and for the target user it will be
    ///  RelationshipType::PendingIncoming.
    ///
    ///  If the current user already has received a game friend request from the target user
    ///  (meaning RelationshipHandle::GameRelationshipType is RelationshipType::PendingIncoming),
    ///  then the two users will become game friends.
    ///
    ///  See RelationshipHandle for more information on the difference between Discord and Game
    ///  relationships.
    ///
    /// </remarks>
    public void SendGameFriendRequest(string username,
                                      SendFriendRequestCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __usernameSpan;
            var __usernameOwned = NativeMethods.__InitStringLocal(
              __scratch, &__scratchUsed, 1024, &__usernameSpan, username);
            NativeMethods.Client.SendFriendRequestCallback __cbDelegate =
          NativeMethods.Client.SendFriendRequestCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendGameFriendRequest(
                  self,
                  __usernameSpan,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
            NativeMethods.__FreeLocal(&__usernameSpan, __usernameOwned);
        }
    }
    /// <summary>
    ///  Sends (or accepts) a game friend request to the target user.
    /// </summary>
    /// <remarks>
    ///  The target user is identified by their Discord ID.
    ///
    ///  After the friend request is sent, each user will have a new game relationship created. For
    ///  the current user the RelationshipHandle::GameRelationshipType will be
    ///  RelationshipType::PendingOutgoing and for the target user it will be
    ///  RelationshipType::PendingIncoming.
    ///
    ///  If the current user already has received a game friend request from the target user
    ///  (meaning RelationshipHandle::GameRelationshipType is RelationshipType::PendingIncoming),
    ///  then the two users will become game friends.
    ///
    ///  See RelationshipHandle for more information on the difference between Discord and Game
    ///  relationships.
    ///
    /// </remarks>
    public void SendGameFriendRequestById(ulong userId,
                                          UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SendGameFriendRequestById(
                  self,
                  userId,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked whenever a relationship for this user is established or
    ///  changes type.
    /// </summary>
    /// <remarks>
    ///  This can be invoked when a user sends or accepts a friend invite or blocks a user for
    ///  example.
    ///
    /// </remarks>
    public void SetRelationshipCreatedCallback(RelationshipCreatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.RelationshipCreatedCallback __cbDelegate =
          NativeMethods.Client.RelationshipCreatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetRelationshipCreatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback to be invoked whenever a relationship for this user is removed,
    ///  such as when the user rejects a friend request or removes a friend.
    /// </summary>
    /// <remarks>
    ///  When a relationship is removed, Client::GetRelationshipHandle will
    ///  return a relationship with the type set to RelationshipType::None.
    ///
    /// </remarks>
    public void SetRelationshipDeletedCallback(RelationshipDeletedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.RelationshipDeletedCallback __cbDelegate =
          NativeMethods.Client.RelationshipDeletedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetRelationshipDeletedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Unblocks the target user. Does not restore any old relationship between the users though.
    /// </summary>
    /// <remarks>
    ///  Fails if the target user is not currently blocked.
    ///
    /// </remarks>
    public void UnblockUser(ulong userId, UpdateRelationshipCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UpdateRelationshipCallback __cbDelegate =
          NativeMethods.Client.UpdateRelationshipCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.UnblockUser(self,
                                                 userId,
                                                 __cbDelegate,
                                                 NativeMethods.ManagedUserData.Free,
                                                 NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Returns the user associated with the current client.
    /// </summary>
    /// <remarks>
    ///  Must not be called before the Client::GetStatus has changed to Status::Ready.
    ///  If the client has disconnected, or is in the process of reconnecting, it will return the
    ///  previous value of the user, even if the auth token has changed since then. Wait for
    ///  client.GetStatus() to change to Ready before accessing it again.
    ///
    /// </remarks>
    public UserHandle GetCurrentUser()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetCurrentUser(self, &__returnValueNative);
            }
            __returnValue = new UserHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  If the Discord app is running on the user's computer and the SDK establishes a connection
    ///  to it, this function will return the user that is currently logged in to the Discord app.
    /// </summary>
    public void GetDiscordClientConnectedUser(
      ulong applicationId,
      GetDiscordClientConnectedUserCallback callback)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client
          .GetDiscordClientConnectedUserCallback __callbackDelegate =
          NativeMethods.Client.GetDiscordClientConnectedUserCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.GetDiscordClientConnectedUser(
                  self,
                  applicationId,
                  __callbackDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(callback));
            }
        }
    }
    /// <summary>
    ///  Returns the UserHandle associated with the given user ID.
    /// </summary>
    /// <remarks>
    ///  It will not fetch a user from Discord's API if it is not available. Generally you can trust
    ///  that users will be available for all relationships and for the authors of any messages
    ///  received.
    ///
    /// </remarks>
    public UserHandle? GetUser(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.Client* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Client.GetUser(self, userId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new UserHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  The UserUpdatedCallback is invoked whenever *any* user the current session knows about
    ///  changes, not just if the current user changes. For example if one of your Discord friends
    ///  changes their name or avatar the UserUpdatedCallback will be invoked. It is also invoked
    ///  when users come online, go offline, or start playing your game.
    /// </summary>
    public void SetUserUpdatedCallback(UserUpdatedCallback cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Client));
        }
        unsafe
        {
            NativeMethods.Client.UserUpdatedCallback __cbDelegate =
          NativeMethods.Client.UserUpdatedCallback_Handler;
            fixed (NativeMethods.Client* self = &this.self)
            {
                NativeMethods.Client.SetUserUpdatedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
}
