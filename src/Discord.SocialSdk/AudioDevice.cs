using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  Represents a single input or output audio device available to the user.
/// </summary>
/// <remarks>
///  Discord will initialize the audio engine with the system default input and output devices.
///  You can change the device through the Client by passing the id of the desired audio device.
///
/// </remarks>
public class AudioDevice : IDisposable
{
    internal NativeMethods.AudioDevice self;
    private int disposed_;

    internal AudioDevice(NativeMethods.AudioDevice self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~AudioDevice() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.Drop(self);
            }
        }
    }

    public AudioDevice(AudioDevice other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.AudioDevice* otherPtr = &other.self)
            {
                fixed (NativeMethods.AudioDevice* selfPtr = &self)
                {
                    NativeMethods.AudioDevice.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe AudioDevice(NativeMethods.AudioDevice* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.AudioDevice* selfPtr = &self)
            {
                NativeMethods.AudioDevice.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Compares the ID of two AudioDevice objects for equality.
    /// </summary>
    public bool Equals(AudioDevice rhs)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.AudioDevice* __rhsFixed = &rhs.self)
            {
                fixed (NativeMethods.AudioDevice* self = &this.self)
                {
                    __returnValue = NativeMethods.AudioDevice.Equals(self, __rhsFixed);
                }
            }
            return __returnValue;
        }
    }
    public string Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.Id(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetId(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.SetId(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string Name()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.Name(self, &__returnValue);
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
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.SetName(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public bool IsDefault()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                __returnValue = NativeMethods.AudioDevice.IsDefault(self);
            }
            return __returnValue;
        }
    }
    public void SetIsDefault(bool value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(AudioDevice));
        }
        unsafe
        {
            fixed (NativeMethods.AudioDevice* self = &this.self)
            {
                NativeMethods.AudioDevice.SetIsDefault(self, value);
            }
        }
    }
}
