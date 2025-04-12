using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Struct that stores information about the channel that a lobby is linked to.
/// </summary>
public class LinkedChannel : IDisposable
{
    internal NativeMethods.LinkedChannel self;
    private int disposed_;

    internal LinkedChannel(NativeMethods.LinkedChannel self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~LinkedChannel() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                NativeMethods.LinkedChannel.Drop(self);
            }
        }
    }

    public LinkedChannel(LinkedChannel other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedChannel* otherPtr = &other.self)
            {
                fixed (NativeMethods.LinkedChannel* selfPtr = &self)
                {
                    NativeMethods.LinkedChannel.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe LinkedChannel(NativeMethods.LinkedChannel* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.LinkedChannel* selfPtr = &self)
            {
                NativeMethods.LinkedChannel.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                __returnValue = NativeMethods.LinkedChannel.Id(self);
            }
            return __returnValue;
        }
    }
    public void SetId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                NativeMethods.LinkedChannel.SetId(self, value);
            }
        }
    }
    public string Name()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                NativeMethods.LinkedChannel.Name(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetName(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                NativeMethods.LinkedChannel.SetName(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public ulong GuildId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                __returnValue = NativeMethods.LinkedChannel.GuildId(self);
            }
            return __returnValue;
        }
    }
    public void SetGuildId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedChannel));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedChannel* self = &this.self)
            {
                NativeMethods.LinkedChannel.SetGuildId(self, value);
            }
        }
    }
}
