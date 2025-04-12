using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  All messages sent on Discord are done so in a Channel. MessageHandle::ChannelId will contain
///  the ID of the channel a message was sent in, and Client::GetChannelHandle will return an
///  instance of this class.
/// </summary>
/// <remarks>
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class ChannelHandle : IDisposable
{
    internal NativeMethods.ChannelHandle self;
    private int disposed_;

    internal ChannelHandle(NativeMethods.ChannelHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ChannelHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.ChannelHandle* self = &this.self)
            {
                NativeMethods.ChannelHandle.Drop(self);
            }
        }
    }

    public ChannelHandle(ChannelHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ChannelHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.ChannelHandle* selfPtr = &self)
                {
                    NativeMethods.ChannelHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ChannelHandle(NativeMethods.ChannelHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ChannelHandle* selfPtr = &self)
            {
                NativeMethods.ChannelHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns the ID of the channel.
    /// </summary>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ChannelHandle* self = &this.self)
            {
                __returnValue = NativeMethods.ChannelHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the name of the channel.
    /// </summary>
    /// <remarks>
    ///  Generally only channels in servers have names, although Discord may generate a display name
    ///  for some channels as well.
    ///
    /// </remarks>
    public string Name()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ChannelHandle* self = &this.self)
            {
                NativeMethods.ChannelHandle.Name(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  For DMs and GroupDMs, returns the user IDs of the members of the channel.
    ///  For all other channels returns an empty list.
    /// </summary>
    public ulong[] Recipients()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed (NativeMethods.ChannelHandle* self = &this.self)
            {
                NativeMethods.ChannelHandle.Recipients(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the type of the channel.
    /// </summary>
    public ChannelType Type()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelHandle));
        }
        unsafe
        {
            ChannelType __returnValue;
            fixed (NativeMethods.ChannelHandle* self = &this.self)
            {
                __returnValue = NativeMethods.ChannelHandle.Type(self);
            }
            return __returnValue;
        }
    }
}
