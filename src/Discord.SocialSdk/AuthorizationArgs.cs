using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Arguments to the Client::Authorize function.
/// </summary>
public class AuthorizationArgs : IDisposable
{
    internal NativeMethods.AuthorizationArgs self;
    private int disposed_;

    internal AuthorizationArgs(NativeMethods.AuthorizationArgs self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~AuthorizationArgs() { Dispose(); }

    public AuthorizationArgs()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.Init(self);
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
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.Drop(self);
            }
        }
    }

    public AuthorizationArgs(AuthorizationArgs other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationArgs* otherPtr = &other.self)
            {
                fixed (NativeMethods.AuthorizationArgs* selfPtr = &self)
                {
                    NativeMethods.AuthorizationArgs.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe AuthorizationArgs(NativeMethods.AuthorizationArgs* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.AuthorizationArgs* selfPtr = &self)
            {
                NativeMethods.AuthorizationArgs.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong ClientId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                __returnValue = NativeMethods.AuthorizationArgs.ClientId(self);
            }
            return __returnValue;
        }
    }
    public void SetClientId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.SetClientId(self, value);
            }
        }
    }
    public string Scopes()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.Scopes(self, &__returnValue);
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
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.SetScopes(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? State()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.AuthorizationArgs.State(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetState(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.SetState(self,
                                                         value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? Nonce()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.AuthorizationArgs.Nonce(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetNonce(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.SetNonce(self,
                                                         value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public AuthorizationCodeChallenge? CodeChallenge()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.AuthorizationCodeChallenge();
            AuthorizationCodeChallenge? __returnValue = null;
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.AuthorizationArgs.CodeChallenge(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new AuthorizationCodeChallenge(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetCodeChallenge(AuthorizationCodeChallenge? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AuthorizationArgs));
        }
        unsafe
        {
            var __valueLocal = value?.self ?? default;
            fixed (NativeMethods.AuthorizationArgs* self = &this.self)
            {
                NativeMethods.AuthorizationArgs.SetCodeChallenge(
                  self, value != null ? &__valueLocal : null);
            }
            if (value != null)
            {
                value.self = __valueLocal;
            }
        }
    }
}
