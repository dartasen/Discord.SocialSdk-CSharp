using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Represents a guild (also knowns as a Discord server), that the current user is a member of,
///  that contains channels that can be linked to a lobby.
/// </summary>
public class GuildMinimal : IDisposable
{
    internal NativeMethods.GuildMinimal self;
    private int disposed_;

    internal GuildMinimal(NativeMethods.GuildMinimal self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~GuildMinimal() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.GuildMinimal* self = &this.self)
            {
                NativeMethods.GuildMinimal.Drop(self);
            }
        }
    }

    public GuildMinimal(GuildMinimal other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(GuildMinimal));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.GuildMinimal* otherPtr = &other.self)
            {
                fixed (NativeMethods.GuildMinimal* selfPtr = &self)
                {
                    NativeMethods.GuildMinimal.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe GuildMinimal(NativeMethods.GuildMinimal* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.GuildMinimal* selfPtr = &self)
            {
                NativeMethods.GuildMinimal.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(GuildMinimal));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.GuildMinimal* self = &this.self)
            {
                __returnValue = NativeMethods.GuildMinimal.Id(self);
            }
            return __returnValue;
        }
    }
    public void SetId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(GuildMinimal));
        }
        unsafe
        {
            fixed (NativeMethods.GuildMinimal* self = &this.self)
            {
                NativeMethods.GuildMinimal.SetId(self, value);
            }
        }
    }
    public string Name()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(GuildMinimal));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.GuildMinimal* self = &this.self)
            {
                NativeMethods.GuildMinimal.Name(self, &__returnValue);
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
            throw new ObjectDisposedException(nameof(GuildMinimal));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.GuildMinimal* self = &this.self)
            {
                NativeMethods.GuildMinimal.SetName(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
