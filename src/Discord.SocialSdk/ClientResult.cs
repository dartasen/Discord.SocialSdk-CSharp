using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Struct that stores information about the result of an SDK function call.
/// </summary>
/// <remarks>
///  Functions can fail for a few reasons including:
///  - The Client is not yet ready and able to perform the action.
///  - The inputs passed to the function are invalid.
///  - The function makes an API call to Discord's backend which returns an error.
///  - The user is offline.
///
///  The ClientResult::Type field is used to to distinguish between the above types of failures
///
/// </remarks>
public class ClientResult : IDisposable
{
    internal NativeMethods.ClientResult self;
    private int disposed_;

    internal ClientResult(NativeMethods.ClientResult self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ClientResult() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.Drop(self);
            }
        }
    }

    public ClientResult(ClientResult other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* otherPtr = &other.self)
            {
                fixed (NativeMethods.ClientResult* selfPtr = &self)
                {
                    NativeMethods.ClientResult.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ClientResult(NativeMethods.ClientResult* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ClientResult* selfPtr = &self)
            {
                NativeMethods.ClientResult.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns the error message if any of the ClientResult.
    /// </summary>
    public override string ToString()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.ToString(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public ErrorType Type()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            ErrorType __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.Type(self);
            }
            return __returnValue;
        }
    }
    public void SetType(ErrorType value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetType(self, value);
            }
        }
    }
    public string Error()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.Error(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetError(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetError(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public int ErrorCode()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            int __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.ErrorCode(self);
            }
            return __returnValue;
        }
    }
    public void SetErrorCode(int value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetErrorCode(self, value);
            }
        }
    }
    public HttpStatusCode Status()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            HttpStatusCode __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.Status(self);
            }
            return __returnValue;
        }
    }
    public void SetStatus(HttpStatusCode value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetStatus(self, value);
            }
        }
    }
    public string ResponseBody()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.ResponseBody(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetResponseBody(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetResponseBody(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public bool Successful()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.Successful(self);
            }
            return __returnValue;
        }
    }
    public void SetSuccessful(bool value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetSuccessful(self, value);
            }
        }
    }
    public bool Retryable()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.Retryable(self);
            }
            return __returnValue;
        }
    }
    public void SetRetryable(bool value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetRetryable(self, value);
            }
        }
    }
    public float RetryAfter()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            float __returnValue;
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                __returnValue = NativeMethods.ClientResult.RetryAfter(self);
            }
            return __returnValue;
        }
    }
    public void SetRetryAfter(float value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ClientResult));
        }
        unsafe
        {
            fixed (NativeMethods.ClientResult* self = &this.self)
            {
                NativeMethods.ClientResult.SetRetryAfter(self, value);
            }
        }
    }
}
