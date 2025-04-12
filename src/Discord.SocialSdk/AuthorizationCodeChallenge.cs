using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Struct that encapsulates the challenge part of the code verification flow.
/// </summary>
public class AuthorizationCodeChallenge : IDisposable
{
    internal NativeMethods.AuthorizationCodeChallenge self;
    private int disposed_;

    internal AuthorizationCodeChallenge(NativeMethods.AuthorizationCodeChallenge self,
                                        int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~AuthorizationCodeChallenge() { Dispose(); }

    public AuthorizationCodeChallenge()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                NativeMethods.AuthorizationCodeChallenge.Init(self);
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
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                NativeMethods.AuthorizationCodeChallenge.Drop(self);
            }
        }
    }

    public AuthorizationCodeChallenge(AuthorizationCodeChallenge other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeChallenge* otherPtr = &other.self)
            {
                fixed (NativeMethods.AuthorizationCodeChallenge* selfPtr = &self)
                {
                    NativeMethods.AuthorizationCodeChallenge.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe AuthorizationCodeChallenge(NativeMethods.AuthorizationCodeChallenge* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeChallenge* selfPtr = &self)
            {
                NativeMethods.AuthorizationCodeChallenge.Clone(selfPtr, otherPtr);
            }
        }
    }
    public AuthenticationCodeChallengeMethod Method()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
        }
        unsafe
        {
            AuthenticationCodeChallengeMethod __returnValue;
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                __returnValue = NativeMethods.AuthorizationCodeChallenge.Method(self);
            }
            return __returnValue;
        }
    }
    public void SetMethod(AuthenticationCodeChallengeMethod value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                NativeMethods.AuthorizationCodeChallenge.SetMethod(self, value);
            }
        }
    }
    public string Challenge()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                NativeMethods.AuthorizationCodeChallenge.Challenge(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetChallenge(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationCodeChallenge));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AuthorizationCodeChallenge* self = &this.self)
            {
                NativeMethods.AuthorizationCodeChallenge.SetChallenge(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
