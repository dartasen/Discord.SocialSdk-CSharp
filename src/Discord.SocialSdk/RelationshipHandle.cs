namespace Discord.SocialSdk;

/// <summary>
///  A RelationshipHandle represents the relationship between the current user and a target user on
///  Discord. Relationships include friends, blocked users, and friend invites.
/// </summary>
/// <remarks>
///  The SDK supports two types of relationships:
///  - Discord: These are relationships that persist across games and on the Discord client.
///  Both users will be able to see whether each other is online regardless of whether they are in
///  the same game or not.
///  - Game: These are per-game relationships and do not carry over to other games. The two users
///  will only be able to see if the other is online if they are playing a game in which they are
///  friends.
///
///  If someone is a game friend they can later choose to "upgrade" to a full Discord friend. In
///  this case, the user has two relationships at the same time, which is why there are two
///  different type fields on RelationshipHandle. In this example, their
///  RelationshipHandle::DiscordRelationshipType would be set to RelationshipType::PendingIncoming
///  or RelationshipType::PendingOutgoing (based on whether they are receiving or sending the invite
///  respectively), and their RelationshipHandle::GameRelationshipType would remain as
///  RelationshipType::Friend.
///
///  When a user blocks another user, it is always stored on the
///  RelationshipHandle::DiscordRelationshipType field, and will persist across games. It is not
///  possible to block a user in only one game.
///
///  See the @ref friends documentation for more information.
///
///  Note: While the SDK allows you to manage a user's relationships, you should never take an
///  action without their explicit consent. You should not automatically send or accept friend
///  requests. Only invoke APIs to manage relationships in response to a user action such as
///  clicking a "Send Friend Request" button.
///
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class RelationshipHandle : IDisposable
{
    internal NativeMethods.RelationshipHandle self;
    private int disposed_;

    internal RelationshipHandle(NativeMethods.RelationshipHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~RelationshipHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.RelationshipHandle* self = &this.self)
            {
                NativeMethods.RelationshipHandle.Drop(self);
            }
        }
    }

    public RelationshipHandle(RelationshipHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.RelationshipHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.RelationshipHandle* selfPtr = &self)
                {
                    NativeMethods.RelationshipHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe RelationshipHandle(NativeMethods.RelationshipHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.RelationshipHandle* selfPtr = &self)
            {
                NativeMethods.RelationshipHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns the type of the Discord relationship.
    /// </summary>
    public RelationshipType DiscordRelationshipType()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        unsafe
        {
            RelationshipType __returnValue;
            fixed (NativeMethods.RelationshipHandle* self = &this.self)
            {
                __returnValue = NativeMethods.RelationshipHandle.DiscordRelationshipType(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the type of the Game relationship.
    /// </summary>
    public RelationshipType GameRelationshipType()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        unsafe
        {
            RelationshipType __returnValue;
            fixed (NativeMethods.RelationshipHandle* self = &this.self)
            {
                __returnValue = NativeMethods.RelationshipHandle.GameRelationshipType(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ID of the target user in this relationship.
    /// </summary>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.RelationshipHandle* self = &this.self)
            {
                __returnValue = NativeMethods.RelationshipHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a handle to the target user in this relationship, if one is available.
    ///  This would be the user with the same ID as the one returned by the Id() method.
    /// </summary>
    public UserHandle? User()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(RelationshipHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.RelationshipHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.RelationshipHandle.User(self, &__returnValueNative);
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
