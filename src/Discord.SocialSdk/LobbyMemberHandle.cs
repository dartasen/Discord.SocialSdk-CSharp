using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  A LobbyMemberHandle represents the state of a single user in a Lobby.
/// </summary>
/// <remarks>
///  The SDK separates lobby membership into two concepts:
///  1. Has the user been added to the lobby by the game developer?
///  If the LobbyMemberHandle exists for a user/lobby pair, then the user has been added to the
///  lobby.
///  2. Does the user have an active game session that is connected to the lobby and will receive
///  any lobby messages? It is possible for a game developer to add a user to a lobby while they are
///  offline. Also users may temporarily disconnect and rejoin later. So the `Connected` boolean
///  tells you whether the user is actively connected to the lobby.
///
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class LobbyMemberHandle : IDisposable
{
    internal NativeMethods.LobbyMemberHandle self;
    private int disposed_;

    internal LobbyMemberHandle(NativeMethods.LobbyMemberHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~LobbyMemberHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                NativeMethods.LobbyMemberHandle.Drop(self);
            }
        }
    }

    public LobbyMemberHandle(LobbyMemberHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.LobbyMemberHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.LobbyMemberHandle* selfPtr = &self)
                {
                    NativeMethods.LobbyMemberHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe LobbyMemberHandle(NativeMethods.LobbyMemberHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.LobbyMemberHandle* selfPtr = &self)
            {
                NativeMethods.LobbyMemberHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns true if the user is allowed to link a channel to this lobby.
    /// </summary>
    /// <remarks>
    ///  Under the hood this checks if the LobbyMemberFlags::CanLinkLobby flag is set.
    ///  This flag can only be set via the server API, add_lobby_member
    ///  The use case for this is for games that want to restrict a lobby so that only the
    ///  clan/guild/group leader is allowed to manage the linked channel for the lobby.
    ///
    /// </remarks>
    public bool CanLinkLobby()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                __returnValue = NativeMethods.LobbyMemberHandle.CanLinkLobby(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns true if the user is currently connected to the lobby.
    /// </summary>
    public bool Connected()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                __returnValue = NativeMethods.LobbyMemberHandle.Connected(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  The user id of the lobby member.
    /// </summary>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                __returnValue = NativeMethods.LobbyMemberHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Metadata is a set of string key/value pairs that the game developer can use.
    /// </summary>
    /// <remarks>
    ///  A common use case may be to store the game's internal user ID for this user so that every
    ///  member of a lobby knows the discord user ID and the game's internal user ID mapping for
    ///  each user.
    ///
    /// </remarks>
    public Dictionary<string, string> Metadata()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.Discord_Properties();
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                NativeMethods.LobbyMemberHandle.Metadata(self, &__returnValueNative);
            }
            var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);
            for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
            {
                var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr,
                                                  (int)__returnValueNative.keys[__i].size);
                var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr,
                                                    (int)__returnValueNative.values[__i].size);
                __returnValue[key] = value;
            }
            NativeMethods.Discord_FreeProperties(__returnValueNative);
            return __returnValue;
        }
    }
    /// <summary>
    ///  The UserHandle of the lobby member.
    /// </summary>
    public UserHandle? User()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyMemberHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.LobbyMemberHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.LobbyMemberHandle.User(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new UserHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
}
