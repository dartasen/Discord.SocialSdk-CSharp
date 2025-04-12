namespace Discord.SocialSdk;

/// <summary>
///  Struct that stores information about the lobby linked to a channel.
/// </summary>
public class LinkedLobby : IDisposable
{
    internal NativeMethods.LinkedLobby self;
    private int disposed_;

    internal LinkedLobby(NativeMethods.LinkedLobby self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~LinkedLobby() { Dispose(); }

    public LinkedLobby()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                NativeMethods.LinkedLobby.Init(self);
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
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                NativeMethods.LinkedLobby.Drop(self);
            }
        }
    }

    public LinkedLobby(LinkedLobby other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedLobby));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedLobby* otherPtr = &other.self)
            {
                fixed (NativeMethods.LinkedLobby* selfPtr = &self)
                {
                    NativeMethods.LinkedLobby.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe LinkedLobby(NativeMethods.LinkedLobby* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.LinkedLobby* selfPtr = &self)
            {
                NativeMethods.LinkedLobby.Clone(selfPtr, otherPtr);
            }
        }
    }
    public ulong ApplicationId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedLobby));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                __returnValue = NativeMethods.LinkedLobby.ApplicationId(self);
            }
            return __returnValue;
        }
    }
    public void SetApplicationId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedLobby));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                NativeMethods.LinkedLobby.SetApplicationId(self, value);
            }
        }
    }
    public ulong LobbyId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedLobby));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                __returnValue = NativeMethods.LinkedLobby.LobbyId(self);
            }
            return __returnValue;
        }
    }
    public void SetLobbyId(ulong value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LinkedLobby));
        }
        unsafe
        {
            fixed (NativeMethods.LinkedLobby* self = &this.self)
            {
                NativeMethods.LinkedLobby.SetLobbyId(self, value);
            }
        }
    }
}
