using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Struct which controls what your rich presence looks like in
///  the Discord client. If you don't specify any values, the icon
///  and name of your application will be used as defaults.
/// </summary>
/// <remarks>
///  Image assets can be either the unique identifier for an image
///  you uploaded to your application via the `Rich Presence` page in
///  the Developer portal, or they can be an external image URL.
///
///  As an example, if I uploaded an asset and name it `goofy-icon`,
///  I could set either image field to the string `goofy-icon`. Alternatively,
///  if my icon was hosted at `http://my-site.com/goofy.jpg`, I could
///  pass that URL into either image field.
///
///  See https://discord.com/developers/docs/rich-presence/overview#adding-custom-art-assets
///  for more information on using custom art assets, as well as for visual
///  examples of what each field does.
///
/// </remarks>
public class ActivityAssets : IDisposable
{
    internal NativeMethods.ActivityAssets self;
    private int disposed_;

    internal ActivityAssets(NativeMethods.ActivityAssets self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ActivityAssets() { Dispose(); }

    public ActivityAssets()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.Init(self);
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
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.Drop(self);
            }
        }
    }

    public ActivityAssets(ActivityAssets other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityAssets* otherPtr = &other.self)
            {
                fixed (NativeMethods.ActivityAssets* selfPtr = &self)
                {
                    NativeMethods.ActivityAssets.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ActivityAssets(NativeMethods.ActivityAssets* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ActivityAssets* selfPtr = &self)
            {
                NativeMethods.ActivityAssets.Clone(selfPtr, otherPtr);
            }
        }
    }
    public string? LargeImage()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.ActivityAssets.LargeImage(self, &__returnValue);
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
    public void SetLargeImage(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.SetLargeImage(self,
                                                           value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? LargeText()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.ActivityAssets.LargeText(self, &__returnValue);
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
    public void SetLargeText(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.SetLargeText(self,
                                                          value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? SmallImage()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.ActivityAssets.SmallImage(self, &__returnValue);
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
    public void SetSmallImage(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.SetSmallImage(self,
                                                           value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? SmallText()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.ActivityAssets.SmallText(self, &__returnValue);
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
    public void SetSmallText(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityAssets));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityAssets* self = &this.self)
            {
                NativeMethods.ActivityAssets.SetSmallText(self,
                                                          value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
}
