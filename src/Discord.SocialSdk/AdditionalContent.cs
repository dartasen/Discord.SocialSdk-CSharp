using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Contains information about non-text content in a message that likely cannot be rendered in game
///  such as images, videos, embeds, polls, and more.
/// </summary>
public class AdditionalContent : IDisposable
{
    internal NativeMethods.AdditionalContent self;
    private int disposed_;

    internal AdditionalContent(NativeMethods.AdditionalContent self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~AdditionalContent() { Dispose(); }

    public AdditionalContent()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                NativeMethods.AdditionalContent.Init(self);
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
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                NativeMethods.AdditionalContent.Drop(self);
            }
        }
    }

    public AdditionalContent(AdditionalContent other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.AdditionalContent* otherPtr = &other.self)
            {
                fixed (NativeMethods.AdditionalContent* selfPtr = &self)
                {
                    NativeMethods.AdditionalContent.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe AdditionalContent(NativeMethods.AdditionalContent* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.AdditionalContent* selfPtr = &self)
            {
                NativeMethods.AdditionalContent.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Compares each field of the AdditionalContent struct for equality.
    /// </summary>
    public bool Equals(AdditionalContent rhs)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.AdditionalContent* __rhsFixed = &rhs.self)
            {
                fixed (NativeMethods.AdditionalContent* self = &this.self)
                {
                    __returnValue = NativeMethods.AdditionalContent.Equals(self, __rhsFixed);
                }
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Converts the AdditionalContentType enum to a string.
    /// </summary>
    public static string TypeToString(AdditionalContentType type)
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.AdditionalContent.TypeToString(type, &__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public AdditionalContentType Type()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            AdditionalContentType __returnValue;
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                __returnValue = NativeMethods.AdditionalContent.Type(self);
            }
            return __returnValue;
        }
    }
    public void SetType(AdditionalContentType value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                NativeMethods.AdditionalContent.SetType(self, value);
            }
        }
    }
    public string? Title()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.AdditionalContent.Title(self, &__returnValue);
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
    public void SetTitle(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                NativeMethods.AdditionalContent.SetTitle(self,
                                                         value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public byte Count()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            byte __returnValue;
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                __returnValue = NativeMethods.AdditionalContent.Count(self);
            }
            return __returnValue;
        }
    }
    public void SetCount(byte value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AdditionalContent));
        }
        unsafe
        {
            fixed (NativeMethods.AdditionalContent* self = &this.self)
            {
                NativeMethods.AdditionalContent.SetCount(self, value);
            }
        }
    }
}
