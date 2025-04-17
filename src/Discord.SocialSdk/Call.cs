using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Class that manages an active voice session in a Lobby.
/// </summary>
public class Call : IDisposable
{
    internal NativeMethods.Call self;
    private int disposed_;

    internal Call(NativeMethods.Call self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~Call() { Dispose(); }

    /// <summary>
    ///  Enum that represents any network errors with the Call.
    /// </summary>
    public enum Error
    {
        None = 0,
        SignalingConnectionFailed = 1,
        SignalingUnexpectedClose = 2,
        VoiceConnectionFailed = 3,
        JoinTimeout = 4,
        Forbidden = 5,
    }

    /// <summary>
    ///  Enum that respresents the state of the Call's network connection.
    /// </summary>
    public enum Status
    {
        Disconnected = 0,
        Joining = 1,
        Connecting = 2,
        SignalingConnected = 3,
        Connected = 4,
        Reconnecting = 5,
        Disconnecting = 6,
    }

    public delegate void OnVoiceStateChanged(ulong userId);
    public delegate void OnParticipantChanged(ulong userId, bool added);
    public delegate void OnSpeakingStatusChanged(ulong userId, bool isPlayingSound);
    public delegate void OnStatusChanged(Status status,
                                         Error error,
                                         int errorDetail);
    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.Drop(self);
            }
        }
    }

    public Call(Call other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.Call* otherPtr = &other.self)
            {
                fixed (NativeMethods.Call* selfPtr = &self)
                {
                    NativeMethods.Call.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe Call(NativeMethods.Call* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.Call* selfPtr = &self)
            {
                NativeMethods.Call.Clone(selfPtr, otherPtr);
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
            NativeMethods.Call.ErrorToString(type, &__returnValue);
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns whether the call is configured to use voice auto detection or push to talk for the
    ///  current user.
    /// </summary>
    public AudioModeType GetAudioMode()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            AudioModeType __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetAudioMode(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ID of the lobby with which this call is associated.
    /// </summary>
    public ulong GetChannelId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetChannelId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ID of the lobby with which this call is associated.
    /// </summary>
    public ulong GetGuildId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetGuildId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether the current user has locally muted the given userId for themselves.
    /// </summary>
    public bool GetLocalMute(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetLocalMute(self, userId);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a list of all of the user IDs of the participants in the call.
    /// </summary>
    public ulong[] GetParticipants()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.GetParticipants(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the locally set playout volume of the given userId.
    /// </summary>
    /// <remarks>
    ///  Does not affect the volume of this user for any other connected clients. The range of
    ///  volume is [0, 200], where 100 indicate default audio volume of the playback device.
    ///
    /// </remarks>
    public float GetParticipantVolume(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            float __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetParticipantVolume(self, userId);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether push to talk is currently active, meaning the user is currently pressing
    ///  their configured push to talk key.
    /// </summary>
    public bool GetPTTActive()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetPTTActive(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the time that PTT is active after the user releases the PTT key and
    ///  SetPTTActive(false) is called.
    /// </summary>
    public uint GetPTTReleaseDelay()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            uint __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetPTTReleaseDelay(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether the current user is deafened.
    /// </summary>
    public bool GetSelfDeaf()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetSelfDeaf(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns whether the current user's microphone is muted.
    /// </summary>
    public bool GetSelfMute()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetSelfMute(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the current call status.
    /// </summary>
    /// <remarks>
    ///  A call is not ready to be used until the status changes to "Connected".
    ///
    /// </remarks>
    public Status GetStatus()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            Status __returnValue;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnValue = NativeMethods.Call.GetStatus(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the current configuration for void auto detection thresholds. See the description
    ///  of the VADThreshold struct for specifics.
    /// </summary>
    public VADThresholdSettings GetVADThreshold()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.VADThresholdSettings();
            VADThresholdSettings? __returnValue = null;
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.GetVADThreshold(self, &__returnValueNative);
            }
            __returnValue = new VADThresholdSettings(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a reference to the VoiceStateHandle for the user ID of the given call participant.
    /// </summary>
    /// <remarks>
    ///  The VoiceStateHandle allows other users to know if the target user has muted or deafened
    ///  themselves.
    ///
    /// </remarks>
    public VoiceStateHandle? GetVoiceStateHandle(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.VoiceStateHandle();
            VoiceStateHandle? __returnValue = null;
            fixed (NativeMethods.Call* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.Call.GetVoiceStateHandle(self, userId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new VoiceStateHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Sets whether to use voice auto detection or push to talk for the current user on this call.
    /// </summary>
    /// <remarks>
    ///  If using push to talk you should call SetPTTActive() whenever the user presses their
    ///  confused push to talk key.
    ///
    /// </remarks>
    public void SetAudioMode(AudioModeType audioMode)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetAudioMode(self, audioMode);
            }
        }
    }
    /// <summary>
    ///  Locally mutes the given userId, so that the current user cannot hear them anymore.
    /// </summary>
    /// <remarks>
    ///  Does not affect whether the given user is muted for any other connected clients.
    ///
    /// </remarks>
    public void SetLocalMute(ulong userId, bool mute)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetLocalMute(self, userId, mute);
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to generally be invoked whenever a field on a VoiceStateHandle
    ///  object for a user would have changed.
    /// </summary>
    /// <remarks>
    ///  For example when a user mutes themselves, all other connected clients will invoke the
    ///  VoiceStateChanged callback, because the "self mute" field will be true now. The callback is
    ///  generally not invoked when users join or leave channels.
    ///
    /// </remarks>
    public void SetOnVoiceStateChangedCallback(OnVoiceStateChanged cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            NativeMethods.Call.OnVoiceStateChanged __cbDelegate =
          NativeMethods.Call.OnVoiceStateChanged_Handler;
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetOnVoiceStateChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever some joins or leaves a voice call.
    /// </summary>
    public void SetParticipantChangedCallback(OnParticipantChanged cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            NativeMethods.Call.OnParticipantChanged __cbDelegate =
          NativeMethods.Call.OnParticipantChanged_Handler;
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetParticipantChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Locally changes the playout volume of the given userId.
    /// </summary>
    /// <remarks>
    ///  Does not affect the volume of this user for any other connected clients. The range of
    ///  volume is [0, 200], where 100 indicate default audio volume of the playback device.
    ///
    /// </remarks>
    public void SetParticipantVolume(ulong userId, float volume)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetParticipantVolume(self, userId, volume);
            }
        }
    }
    /// <summary>
    ///  When push to talk is enabled, this should be called whenever the user pushes or releases
    ///  their configured push to talk key. This key must be configured in the game, the SDK does
    ///  not handle keybinds itself.
    /// </summary>
    public void SetPTTActive(bool active)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetPTTActive(self, active);
            }
        }
    }
    /// <summary>
    ///  If set, extends the time that PTT is active after the user releases the PTT key and
    ///  SetPTTActive(false) is called.
    /// </summary>
    /// <remarks>
    ///  Defaults to no release delay, but we recommend setting to 20ms, which is what Discord uses.
    ///
    /// </remarks>
    public void SetPTTReleaseDelay(uint releaseDelayMs)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetPTTReleaseDelay(self, releaseDelayMs);
            }
        }
    }
    /// <summary>
    ///  Mutes all audio from the currently active call for the current user.
    ///  They will not be able to hear any other participants,
    ///  and no other participants will be able to hear the current user either.
    /// </summary>
    public void SetSelfDeaf(bool deaf)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetSelfDeaf(self, deaf);
            }
        }
    }
    /// <summary>
    ///  Mutes the current user's microphone so that no other participant in their active calls can
    ///  hear them.
    /// </summary>
    public void SetSelfMute(bool mute)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetSelfMute(self, mute);
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked whenever a user starts or stops speaking and is
    ///  passed in the userId and whether they are currently speaking.
    /// </summary>
    /// <remarks>
    ///  It can be invoked in other cases as well, such as if the priority speaker changes or if the
    ///  user plays a soundboard sound.
    ///
    /// </remarks>
    public void SetSpeakingStatusChangedCallback(OnSpeakingStatusChanged cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            NativeMethods.Call.OnSpeakingStatusChanged __cbDelegate =
          NativeMethods.Call.OnSpeakingStatusChanged_Handler;
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetSpeakingStatusChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Sets a callback function to be invoked when the call status changes, such as when it fully
    ///  connects or starts reconnecting.
    /// </summary>
    public void SetStatusChangedCallback(OnStatusChanged cb)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            NativeMethods.Call.OnStatusChanged __cbDelegate =
          NativeMethods.Call.OnStatusChanged_Handler;
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetStatusChangedCallback(
                  self,
                  __cbDelegate,
                  NativeMethods.ManagedUserData.Free,
                  NativeMethods.ManagedUserData.CreateHandle(cb));
            }
        }
    }
    /// <summary>
    ///  Customizes the void auto detection thresholds for picking up activity from a user's mic.
    ///  - When automatic is set to True, Discord will automatically detect the appropriate
    ///  threshold to use.
    ///  - When automatic is set to False, the given threshold value will be used. Threshold has a
    ///  range of -100, 0, and defaults to -60.
    /// </summary>
    public void SetVADThreshold(bool automatic, float threshold)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Call));
        }
        unsafe
        {
            fixed (NativeMethods.Call* self = &this.self)
            {
                NativeMethods.Call.SetVADThreshold(self, automatic, threshold);
            }
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
            NativeMethods.Call.StatusToString(type, &__returnValue);
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
}
