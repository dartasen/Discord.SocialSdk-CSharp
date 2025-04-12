using System;

namespace Discord.SocialSdk;

/// <summary>
///  Convenience class that represents the state of a single Discord call in a lobby.
/// </summary>
public class CallInfoHandle : IDisposable
{
    internal NativeMethods.CallInfoHandle self;
    private int disposed_;

    internal CallInfoHandle(NativeMethods.CallInfoHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~CallInfoHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.CallInfoHandle* self = &this.self)
            {
                NativeMethods.CallInfoHandle.Drop(self);
            }
        }
    }

    public CallInfoHandle(CallInfoHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(CallInfoHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.CallInfoHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.CallInfoHandle* selfPtr = &self)
                {
                    NativeMethods.CallInfoHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe CallInfoHandle(NativeMethods.CallInfoHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.CallInfoHandle* selfPtr = &self)
            {
                NativeMethods.CallInfoHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns the lobby ID of the call.
    /// </summary>
    public ulong ChannelId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(CallInfoHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.CallInfoHandle* self = &this.self)
            {
                __returnValue = NativeMethods.CallInfoHandle.ChannelId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a list of the user IDs of the participants in the call.
    /// </summary>
    public ulong[] GetParticipants()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(CallInfoHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed (NativeMethods.CallInfoHandle* self = &this.self)
            {
                NativeMethods.CallInfoHandle.GetParticipants(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Accesses the voice state for a single user so you can know if they have muted or deafened
    ///  themselves.
    /// </summary>
    public VoiceStateHandle? GetVoiceStateHandle(ulong userId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(CallInfoHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.VoiceStateHandle();
            VoiceStateHandle? __returnValue = null;
            fixed (NativeMethods.CallInfoHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.CallInfoHandle.GetVoiceStateHandle(
                  self, userId, &__returnValueNative);
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
    ///  Returns the lobby ID of the call.
    /// </summary>
    public ulong GuildId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(CallInfoHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.CallInfoHandle* self = &this.self)
            {
                __returnValue = NativeMethods.CallInfoHandle.GuildId(self);
            }
            return __returnValue;
        }
    }
}
