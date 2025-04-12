namespace Discord.SocialSdk;

/// <summary>
///  Settings for the void auto detection threshold for picking up activity from a user's mic.
/// </summary>
public class VADThresholdSettings : IDisposable
{
    internal NativeMethods.VADThresholdSettings self;
    private int disposed_;

    internal VADThresholdSettings(NativeMethods.VADThresholdSettings self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~VADThresholdSettings() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.VADThresholdSettings* self = &this.self)
            {
                NativeMethods.VADThresholdSettings.Drop(self);
            }
        }
    }
    public float VadThreshold()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VADThresholdSettings));
        }
        unsafe
        {
            float __returnValue;
            fixed (NativeMethods.VADThresholdSettings* self = &this.self)
            {
                __returnValue = NativeMethods.VADThresholdSettings.VadThreshold(self);
            }
            return __returnValue;
        }
    }
    public void SetVadThreshold(float value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VADThresholdSettings));
        }
        unsafe
        {
            fixed (NativeMethods.VADThresholdSettings* self = &this.self)
            {
                NativeMethods.VADThresholdSettings.SetVadThreshold(self, value);
            }
        }
    }
    public bool Automatic()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VADThresholdSettings));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.VADThresholdSettings* self = &this.self)
            {
                __returnValue = NativeMethods.VADThresholdSettings.Automatic(self);
            }
            return __returnValue;
        }
    }
    public void SetAutomatic(bool value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(VADThresholdSettings));
        }
        unsafe
        {
            fixed (NativeMethods.VADThresholdSettings* self = &this.self)
            {
                NativeMethods.VADThresholdSettings.SetAutomatic(self, value);
            }
        }
    }
}
