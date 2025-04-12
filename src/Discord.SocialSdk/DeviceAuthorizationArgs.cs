using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Arguments to the Client::GetTokenFromDevice function.
/// </summary>
public class DeviceAuthorizationArgs : IDisposable
{
    internal NativeMethods.DeviceAuthorizationArgs self;
    private int disposed_;

    internal DeviceAuthorizationArgs(NativeMethods.DeviceAuthorizationArgs self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~DeviceAuthorizationArgs() { Dispose(); }

    public DeviceAuthorizationArgs()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                NativeMethods.DeviceAuthorizationArgs.Init(self);
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
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                NativeMethods.DeviceAuthorizationArgs.Drop(self);
            }
        }
    }

    public DeviceAuthorizationArgs(DeviceAuthorizationArgs other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* otherPtr = &other.self)
            {
                fixed (NativeMethods.DeviceAuthorizationArgs* selfPtr = &self)
                {
                    NativeMethods.DeviceAuthorizationArgs.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe DeviceAuthorizationArgs(NativeMethods.DeviceAuthorizationArgs* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* selfPtr = &self)
            {
                NativeMethods.DeviceAuthorizationArgs.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong ClientId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                __returnValue = NativeMethods.DeviceAuthorizationArgs.ClientId(self);
            }
            return __returnValue;
        }
    }
    public void SetClientId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
        }
        unsafe
        {
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                NativeMethods.DeviceAuthorizationArgs.SetClientId(self, value);
            }
        }
    }
    public string Scopes()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                NativeMethods.DeviceAuthorizationArgs.Scopes(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetScopes(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(DeviceAuthorizationArgs));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.DeviceAuthorizationArgs* self = &this.self)
            {
                NativeMethods.DeviceAuthorizationArgs.SetScopes(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
