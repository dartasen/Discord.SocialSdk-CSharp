using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
/// \see Activity
/// </summary>
public class ActivitySecrets : IDisposable
{
    internal NativeMethods.ActivitySecrets self;
    private int disposed_;

    internal ActivitySecrets(NativeMethods.ActivitySecrets self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ActivitySecrets() { Dispose(); }

    public ActivitySecrets()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.ActivitySecrets* self = &this.self)
            {
                NativeMethods.ActivitySecrets.Init(self);
            }
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
            fixed (NativeMethods.ActivitySecrets* self = &this.self)
            {
                NativeMethods.ActivitySecrets.Drop(self);
            }
        }
    }

    public ActivitySecrets(ActivitySecrets other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivitySecrets));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ActivitySecrets* otherPtr = &other.self)
            {
                fixed (NativeMethods.ActivitySecrets* selfPtr = &self)
                {
                    NativeMethods.ActivitySecrets.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ActivitySecrets(NativeMethods.ActivitySecrets* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ActivitySecrets* selfPtr = &self)
            {
                NativeMethods.ActivitySecrets.Clone(selfPtr, otherPtr);
            }
        }
    }
    public string Join()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivitySecrets));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivitySecrets* self = &this.self)
            {
                NativeMethods.ActivitySecrets.Join(self, &__returnValue);
            }
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetJoin(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivitySecrets));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivitySecrets* self = &this.self)
            {
                NativeMethods.ActivitySecrets.SetJoin(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
