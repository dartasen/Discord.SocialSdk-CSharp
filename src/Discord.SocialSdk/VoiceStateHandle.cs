namespace Discord.SocialSdk;

/// <summary>
///  A VoiceStateHandle represents the state of a single participant in a Discord voice call.
/// </summary>
/// <remarks>
///  The main use case for VoiceStateHandle in the SDK is communicate whether a user has muted or
///  defeaned themselves.
///
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class VoiceStateHandle : IDisposable
{
    internal NativeMethods.VoiceStateHandle self;
    private int disposed_;

    internal VoiceStateHandle(NativeMethods.VoiceStateHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~VoiceStateHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.VoiceStateHandle* self = &this.self)
            {
                NativeMethods.VoiceStateHandle.Drop(self);
            }
        }
    }

    public VoiceStateHandle(VoiceStateHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VoiceStateHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.VoiceStateHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.VoiceStateHandle* selfPtr = &self)
                {
                    NativeMethods.VoiceStateHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe VoiceStateHandle(NativeMethods.VoiceStateHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.VoiceStateHandle* selfPtr = &self)
            {
                NativeMethods.VoiceStateHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns true if the given user has deafened themselves so that no one else in the call can
    ///  hear them and so that they do not hear anyone else in the call either.
    /// </summary>
    public bool SelfDeaf()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VoiceStateHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.VoiceStateHandle* self = &this.self)
            {
                __returnValue = NativeMethods.VoiceStateHandle.SelfDeaf(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns true if the given user has muted themselves so that no one else in the call can
    ///  hear them.
    /// </summary>
    public bool SelfMute()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VoiceStateHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.VoiceStateHandle* self = &this.self)
            {
                __returnValue = NativeMethods.VoiceStateHandle.SelfMute(self);
            }
            return __returnValue;
        }
    }
}
