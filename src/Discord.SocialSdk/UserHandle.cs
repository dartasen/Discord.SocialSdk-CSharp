using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  A UserHandle represents a single user on Discord that the SDK knows about and contains basic
///  account information for them such as id, name, and avatar, as well as their "status"
///  information which includes both whether they are online/offline/etc as well as whether they are
///  playing this game.
/// </summary>
/// <remarks>
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class UserHandle : IDisposable
{
    internal NativeMethods.UserHandle self;
    private int disposed_;

    internal UserHandle(NativeMethods.UserHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~UserHandle() { Dispose(); }

    /// <summary>
    ///  The desired type of avatar url to generate for a User.
    /// </summary>
    public enum AvatarType
    {
        Gif = 0,
        Webp = 1,
        Png = 2,
        Jpeg = 3,
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
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                NativeMethods.UserHandle.Drop(self);
            }
        }
    }

    public UserHandle(UserHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.UserHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.UserHandle* selfPtr = &self)
                {
                    NativeMethods.UserHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe UserHandle(NativeMethods.UserHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.UserHandle* selfPtr = &self)
            {
                NativeMethods.UserHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Returns the hash of the user's Discord profile avatar, if one is set.
    /// </summary>
    public string? Avatar()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.UserHandle.Avatar(self, &__returnValue);
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
    /// <summary>
    ///  Converts the AvatarType enum to a string.
    /// </summary>
    public static string AvatarTypeToString(AvatarType type)
    {
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            NativeMethods.UserHandle.AvatarTypeToString(type, &__returnValue);
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns a CDN url to the user's Discord profile avatar.
    /// </summary>
    /// <remarks>
    ///  If the user does not have an avatar set, a url to one of Discord's default avatars is
    ///  returned instead.
    ///
    /// </remarks>
    public string AvatarUrl(AvatarType animatedType,
                            AvatarType staticType)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                NativeMethods.UserHandle.AvatarUrl(self, animatedType, staticType, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the user's preferred name, if one is set, otherwise returns their unique username.
    /// </summary>
    public string DisplayName()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                NativeMethods.UserHandle.DisplayName(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the user's rich presence activity that is associated with the current game, if one
    ///  is set.
    /// </summary>
    /// <remarks>
    ///  On Discord, users can have multiple rich presence activities at once, but the SDK will only
    ///  expose the activity that is associated with your game. You can use this to know about the
    ///  party the user is in, if any, and what the user is doing in the game.
    ///
    ///  For more information see the Activity class and check out
    ///  https://discord.com/developers/docs/rich-presence/overview
    ///
    /// </remarks>
    public Activity? GameActivity()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.Activity();
            Activity? __returnValue = null;
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.UserHandle.GameActivity(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new Activity(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the preferred display name of this user, if one is set.
    /// </summary>
    /// <remarks>
    ///  Discord's public API refers to this as a "global name" instead of "display name".
    ///
    ///  Discord users can set their preferred name to almost any string.
    ///
    ///  For more information about usernames on Discord, see:
    ///  https://discord.com/developers/docs/resources/user
    ///
    /// </remarks>
    public string? GlobalName()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.UserHandle.GlobalName(self, &__returnValue);
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
    /// <summary>
    ///  Returns the ID of this user.
    /// </summary>
    /// <remarks>
    ///  If this returns 0 then the UserHandle is no longer valid.
    ///
    /// </remarks>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnValue = NativeMethods.UserHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns true if this user is a provisional account.
    /// </summary>
    public bool IsProvisional()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnValue = NativeMethods.UserHandle.IsProvisional(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns a reference to the RelationshipHandle between the currently authenticated user and
    ///  this user, if any.
    /// </summary>
    public RelationshipHandle Relationship()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.RelationshipHandle();
            RelationshipHandle? __returnValue = null;
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                NativeMethods.UserHandle.Relationship(self, &__returnValueNative);
            }
            __returnValue = new RelationshipHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the user's online/offline/idle status.
    /// </summary>
    public StatusType Status()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            StatusType __returnValue;
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                __returnValue = NativeMethods.UserHandle.Status(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the globally unique username of this user.
    /// </summary>
    /// <remarks>
    ///  For provisional accounts this is an auto-generated string.
    ///
    ///  For more information about usernames on Discord, see:
    ///  https://discord.com/developers/docs/resources/user
    ///
    /// </remarks>
    public string Username()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(UserHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.UserHandle* self = &this.self)
            {
                NativeMethods.UserHandle.Username(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
}
