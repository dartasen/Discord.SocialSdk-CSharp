namespace Discord.SocialSdk;

/// <summary>
/// \see Activity
/// </summary>
public class ActivityTimestamps : IDisposable
{
    internal NativeMethods.ActivityTimestamps self;
    private int disposed_;

    internal ActivityTimestamps(NativeMethods.ActivityTimestamps self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ActivityTimestamps() { Dispose(); }

    public ActivityTimestamps()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                NativeMethods.ActivityTimestamps.Init(self);
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
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                NativeMethods.ActivityTimestamps.Drop(self);
            }
        }
    }

    public ActivityTimestamps(ActivityTimestamps other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityTimestamps));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityTimestamps* otherPtr = &other.self)
            {
                fixed (NativeMethods.ActivityTimestamps* selfPtr = &self)
                {
                    NativeMethods.ActivityTimestamps.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ActivityTimestamps(NativeMethods.ActivityTimestamps* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ActivityTimestamps* selfPtr = &self)
            {
                NativeMethods.ActivityTimestamps.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong Start()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityTimestamps));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityTimestamps.Start(self);
            }
            return __returnValue;
        }
    }
    public void SetStart(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityTimestamps));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                NativeMethods.ActivityTimestamps.SetStart(self, value);
            }
        }
    }
    public ulong End()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityTimestamps));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityTimestamps.End(self);
            }
            return __returnValue;
        }
    }
    public void SetEnd(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityTimestamps));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityTimestamps* self = &this.self)
            {
                NativeMethods.ActivityTimestamps.SetEnd(self, value);
            }
        }
    }
}
