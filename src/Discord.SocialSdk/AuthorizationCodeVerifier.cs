using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Struct that encapsulates both parts of the code verification flow.
/// </summary>
public class AuthorizationCodeVerifier : IDisposable
{
    internal NativeMethods.AuthorizationCodeVerifier self;
    private int disposed_;

    internal AuthorizationCodeVerifier(NativeMethods.AuthorizationCodeVerifier self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~AuthorizationCodeVerifier() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
            {
                NativeMethods.AuthorizationCodeVerifier.Drop(self);
            }
        }
    }

    public AuthorizationCodeVerifier(AuthorizationCodeVerifier other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeVerifier* otherPtr = &other.self)
            {
                fixed (NativeMethods.AuthorizationCodeVerifier* selfPtr = &self)
                {
                    NativeMethods.AuthorizationCodeVerifier.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe AuthorizationCodeVerifier(NativeMethods.AuthorizationCodeVerifier* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeVerifier* selfPtr = &self)
            {
                NativeMethods.AuthorizationCodeVerifier.Clone(selfPtr, otherPtr);
            }
        }
    }
    public AuthorizationCodeChallenge Challenge()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.AuthorizationCodeChallenge();
            AuthorizationCodeChallenge? __returnValue = null;
            fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
            {
                NativeMethods.AuthorizationCodeVerifier.Challenge(self, &__returnValueNative);
            }
            __returnValue = new AuthorizationCodeChallenge(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetChallenge(AuthorizationCodeChallenge value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeChallenge* __valueFixed = &value.self)
            {
                fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
                {
                    NativeMethods.AuthorizationCodeVerifier.SetChallenge(self, __valueFixed);
                }
            }
        }
    }
    public string Verifier()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
            {
                NativeMethods.AuthorizationCodeVerifier.Verifier(self, &__returnValue);
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
    public void SetVerifier(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeVerifier));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AuthorizationCodeVerifier* self = &this.self)
            {
                NativeMethods.AuthorizationCodeVerifier.SetVerifier(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
