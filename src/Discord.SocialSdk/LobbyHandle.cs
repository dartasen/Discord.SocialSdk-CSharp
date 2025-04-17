using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  A LobbyHandle represents a single lobby in the SDK. A lobby can be thought of as
///  just an arbitrary, developer-controlled group of users that can communicate with each other.
/// </summary>
/// <remarks>
///  ## Managing Lobbies
///  Lobbies can be managed through a set of @ref server_apis that are documented elsewhere, which
///  allow you to create lobbies, add and remove users from lobbies, and delete them.
///
///  There is also an API to create lobbies without any server side component using the
///  Client::CreateOrJoinLobby function, which accepts a game-generated secret and will join the
///  user to the lobby associated with that secret, creating it if necessary.
///
///  NOTE: When using this API the secret will auto-expire in 30 days, at which point the existing
///  lobby can no longer be joined, but will still exist. We recommend using this for short term
///  lobbies and not permanent lobbies. Use the Server API for more permanent lobbies.
///
///  Members of a lobby are not automatically removed when they close the game or temporarily
///  disconnect. When the SDK connects, it will attempt to re-connect to any lobbies it is currently
///  a member of.
///
///  # Lobby Auto-Deletion
///  Lobbies are generally ephemeral and will be auto-deleted if they have been idle (meaning no
///  users are actively connected to them) for some amount of time. The default is to auto delete
///  after 5 minutes, but this can be customized when creating the lobby. As long as one user is
///  connected to the lobby though it will not be auto-deleted (meaning they have the SDK running
///  and status is set to Ready). Additionally, lobbies that are linked to a channel on Discord will
///  not be auto deleted.
///
///  You can also use the @ref server_apis to customize this timeout, it can be raised to as high as
///  7 days, meaning the lobby only gets deleted if no one connects to it for an entire week. This
///  should give a good amount of permanence to lobbies when needed, but there may be rare cases
///  where a lobby does need to be "rebuilt" if everyone is offline for an extended period.
///
///  # Membership Limits
///  Lobbies may have a maximum of 1,000 members, and each user may be in a maximum of 100 lobbies
///  per game.
///
///  ## Audio
///  Lobbies support voice calls. Although a lobby is allowed to have 1,000 members, you should not
///  try to start voice calls in lobbies that large. We strongly recommend sticking to around 25
///  members or fewer for voice calls.
///
///  See Client::StartCall for more information on how to start a voice call in a lobby.
///
///  ## Channel Linking
///  Lobbies can be linked to a channel on Discord, which allows messages sent in one place to show
///  up in the other. Any lobby can be linked to a channel, but only lobby members with the
///  LobbyMemberFlags::CanLinkLobby flag are allowed to a link a lobby. This flag must be set using
///  the server APIs, which allows you to ensure that only clan/guild/group leaders can link lobbies
///  to Discord channels.
///
///  To setup a link you'll need to use methods in the Client class to fetch the servers (aka
///  guilds) and channels a user is a member of and setup the link. The Client::GetUserGuilds and
///  Client::GetGuildChannels methods are used to fetch the servers and channels respectively. You
///  can use these to show a UI for the user to pick which server and channel they want to link to.
///
///  Not all channels are linkable. To be linked:
///  - The channel must be a guild text channel
///  - The channel may not be marked as NSFW
///  - The channel must not be currently linked to a different lobby
///  - The user must have the following permissions in the channel in order to link it:
///    - Manage Channels
///    - View Channel
///    - Send Messages
///
///  ### Linking Private Channels
///  Discord is allowing all channels the user has access to in a server to be linked in game, even
///  if that channel is private to other members of the server. This means that a user could choose
///  to link a private "admins chat" channel (assuming they are an admin) in game if they wanted.
///
///  It's not really possible for the game to know which users should have access to that channel or
///  not though. So in this implementation, every member of a lobby will be able to view all
///  messages sent in the linked channel and reply to them. If you are going to allow private
///  channels to be linked in game, you must make sure that users are aware that their private
///  channel will be viewable by everyone in the lobby!
///
///  To help you identify which channels are public or private, we have added a
///  isViewableAndWriteableByAllMembers boolean which is described more in GuildChannel. You can use
///  that to just not allow private channels to be linked, or to know when to show a clear warning,
///  it's up to you!
///
///  ## Misc
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class LobbyHandle : IDisposable
{
    internal NativeMethods.LobbyHandle self;
    private int disposed_;

    internal LobbyHandle(NativeMethods.LobbyHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~LobbyHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                NativeMethods.LobbyHandle.Drop(self);
            }
        }
    }

    public LobbyHandle(LobbyHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.LobbyHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.LobbyHandle* selfPtr = &self)
                {
                    NativeMethods.LobbyHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe LobbyHandle(NativeMethods.LobbyHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.LobbyHandle* selfPtr = &self)
            {
                NativeMethods.LobbyHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns a reference to the CallInfoHandle if there is an active voice call in this lobby.
    /// </summary>
    /// <remarks>
    ///  This can allow you to display which lobby members are in voice, even if the current user
    ///  has not yet joined the voice call.
    ///
    /// </remarks>
    public CallInfoHandle? GetCallInfoHandle()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.CallInfoHandle();
            CallInfoHandle? __returnValue = null;
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.LobbyHandle.GetCallInfoHandle(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new CallInfoHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a reference to the LobbyMemberHandle for the given user ID, if they are a member of
    ///  this lobby.
    /// </summary>
    public LobbyMemberHandle? GetLobbyMemberHandle(ulong memberId)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.LobbyMemberHandle();
            LobbyMemberHandle? __returnValue = null;
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.LobbyHandle.GetLobbyMemberHandle(
                  self, memberId, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new LobbyMemberHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the id of the lobby.
    /// </summary>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                __returnValue = NativeMethods.LobbyHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns information about the channel linked to this lobby, if any.
    /// </summary>
    public LinkedChannel? LinkedChannel()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.LinkedChannel();
            LinkedChannel? __returnValue = null;
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.LobbyHandle.LinkedChannel(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new LinkedChannel(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a list of the user IDs that are members of this lobby.
    /// </summary>
    public ulong[] LobbyMemberIds()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_UInt64Span();
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                NativeMethods.LobbyHandle.LobbyMemberIds(self, &__returnValue);
            }
            var __returnValueSurface =
              new Span<ulong>(__returnValue.ptr, (int)__returnValue.size).ToArray();
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns a list of the LobbyMemberHandle objects for each member of this lobby.
    /// </summary>
    public LobbyMemberHandle[] LobbyMembers()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_LobbyMemberHandleSpan();
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                NativeMethods.LobbyHandle.LobbyMembers(self, &__returnValue);
            }
            var __returnValueSurface = new LobbyMemberHandle[(int)__returnValue.size];
            for (int __i = 0; __i < (int)__returnValue.size; __i++)
            {
                __returnValueSurface[__i] = new LobbyMemberHandle(__returnValue.ptr[__i], 0);
            }
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns any developer supplied metadata for this lobby.
    /// </summary>
    /// <remarks>
    ///  Metadata is simple string key/value pairs and is a way to associate internal game
    ///  information with the lobby so each lobby member can have easy access to.
    ///
    /// </remarks>
    public Dictionary<string, string> Metadata()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(LobbyHandle));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.Discord_Properties();
            fixed (NativeMethods.LobbyHandle* self = &this.self)
            {
                NativeMethods.LobbyHandle.Metadata(self, &__returnValueNative);
            }
            var __returnValue = new Dictionary<string, string>((int)__returnValueNative.size);
            for (int __i = 0; __i < (int)__returnValueNative.size; __i++)
            {
#if NETSTANDARD2_0
                var key = MarshalP.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr, (int)__returnValueNative.keys[__i].size);
                var value = MarshalP.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr, (int)__returnValueNative.values[__i].size);
#else
                var key = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.keys[__i].ptr, (int)__returnValueNative.keys[__i].size);
                var value = Marshal.PtrToStringUTF8((IntPtr)__returnValueNative.values[__i].ptr, (int)__returnValueNative.values[__i].size);
#endif
                __returnValue[key] = value;
            }

            NativeMethods.Discord_FreeProperties(__returnValueNative);
            return __returnValue;
        }
    }
}
