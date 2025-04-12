using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  When one user invites another to join their game on Discord, it will send a message to that
///  user. The SDK will parse those messages for you automatically, and this struct contains all of
///  the relevant invite information which is needed to later accept that invite.
/// </summary>
public class ActivityInvite : IDisposable
{
    internal NativeMethods.ActivityInvite self;
    private int disposed_;

    internal ActivityInvite(NativeMethods.ActivityInvite self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~ActivityInvite() { Dispose(); }

    public ActivityInvite()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.Init(self);
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
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.Drop(self);
            }
        }
    }

    public ActivityInvite(ActivityInvite other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* otherPtr = &other.self)
            {
                fixed (NativeMethods.ActivityInvite* selfPtr = &self)
                {
                    NativeMethods.ActivityInvite.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe ActivityInvite(NativeMethods.ActivityInvite* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* selfPtr = &self)
            {
                NativeMethods.ActivityInvite.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong SenderId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.SenderId(self);
            }
            return __returnValue;
        }
    }
    public void SetSenderId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetSenderId(self, value);
            }
        }
    }
    public ulong ChannelId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.ChannelId(self);
            }
            return __returnValue;
        }
    }
    public void SetChannelId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetChannelId(self, value);
            }
        }
    }
    public ulong MessageId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.MessageId(self);
            }
            return __returnValue;
        }
    }
    public void SetMessageId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetMessageId(self, value);
            }
        }
    }
    public ActivityActionTypes Type()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            ActivityActionTypes __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.Type(self);
            }
            return __returnValue;
        }
    }
    public void SetType(ActivityActionTypes value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetType(self, value);
            }
        }
    }
    public ulong ApplicationId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.ApplicationId(self);
            }
            return __returnValue;
        }
    }
    public void SetApplicationId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetApplicationId(self, value);
            }
        }
    }
    public string PartyId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.PartyId(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetPartyId(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetPartyId(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string SessionId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SessionId(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetSessionId(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetSessionId(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public bool IsValid()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                __returnValue = NativeMethods.ActivityInvite.IsValid(self);
            }
            return __returnValue;
        }
    }
    public void SetIsValid(bool value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(ActivityInvite));
        }
        unsafe
        {
            fixed (NativeMethods.ActivityInvite* self = &this.self)
            {
                NativeMethods.ActivityInvite.SetIsValid(self, value);
            }
        }
    }
}
