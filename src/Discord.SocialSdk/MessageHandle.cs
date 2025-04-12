using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  A MessageHandle represents a single message received by the SDK.
/// </summary>
/// <remarks>
///  # Chat types
///  The SDK supports two types of chat:
///  1. 1 on 1 chat between two users
///  2. Chat within a lobby
///
///  You can determine the context a message was sent in with the MessageHandle::Channel and
///  ChannelHandle::Type methods. The SDK should only be receiving messages in the following channel
///  types:
///  - DM
///  - Ephemeral DM
///  - Lobby
///
///  # Syncing with Discord
///  In some situations messages sent from the SDK will also show up in Discord.
///  In general this will happen for:
///  - 1 on 1 chat when at least one of the users is a full Discord user
///  - Lobby chat when the lobby is linked to a Discord channel
///
///  Additionally the message must have been sent by a user who is not banned on the Discord side.
///
///  # Legal disclosures
///  As a convenience for game developers, the first time a user sends a message in game, and that
///  message will show up on the Discord client, the SDK will inject a "fake" message into the chat,
///  that contains a basic English explanation of what is happening to the user. You can identify
///  these messages with the MessageHandle::DisclosureType method. We encourage you to customize the
///  rendering of these messages, possibly changing the wording, translating them, and making them
///  look more "official". You can choose to avoid rendering these as well.
///
///  # History
///  The SDK keeps the 25 most recent messages in each channel in memory, but it does not have
///  access to any historical messages sent before the SDK was connected. A MessageHandle will keep
///  working though even after the SDK has discarded the message for being too old, you just won't
///  be able to create a new MessageHandle objects for that message.
///
///  # Unrenderable Content
///  Messages sent on Discord can contain content that may not be renderable in game, such as
///  images, videos, embeds, polls, and more. The game isn't expected to render these, but instead
///  show a small notice so the user is aware there is more content and a way to view that content
///  on Discord. The MessageHandle::AdditionalContent method will contain data about the non-text
///  content in this message.
///
///  There is also more information about the struct of messages on Discord here:
///  https://discord.com/developers/docs/resources/message
///
///  Note: While the SDK allows you to send messages on behalf of a user, you must only do so in
///  response to a user action. You should never automatically send messages.
///
///  Handle objects in the SDK hold a reference both to the underlying data, and to the SDK
///  instance. Changes to the underlying data will generally be available on existing handles
///  objects without having to re-create them. If the SDK instance is destroyed, but you still have
///  a reference to a handle object, note that it will return the default value for all method calls
///  (ie an empty string for methods that return a string).
///
/// </remarks>
public class MessageHandle : IDisposable
{
    internal NativeMethods.MessageHandle self;
    private int disposed_;

    internal MessageHandle(NativeMethods.MessageHandle self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~MessageHandle() { Dispose(); }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed_, 1) != 0)
        {
            return;
        }
        GC.SuppressFinalize(this);
        unsafe
        {
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                NativeMethods.MessageHandle.Drop(self);
            }
        }
    }

    public MessageHandle(MessageHandle other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.MessageHandle* otherPtr = &other.self)
            {
                fixed (NativeMethods.MessageHandle* selfPtr = &self)
                {
                    NativeMethods.MessageHandle.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe MessageHandle(NativeMethods.MessageHandle* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.MessageHandle* selfPtr = &self)
            {
                NativeMethods.MessageHandle.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  If the message contains non-text content, such as images, videos, embeds, polls, etc, this
    ///  method will return information about that content.
    /// </summary>
    public AdditionalContent? AdditionalContent()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.AdditionalContent();
            AdditionalContent? __returnValue = null;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.MessageHandle.AdditionalContent(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new AdditionalContent(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the UserHandle for the author of this message.
    /// </summary>
    public UserHandle? Author()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.MessageHandle.Author(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new UserHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the user ID of the user who sent this message.
    /// </summary>
    public ulong AuthorId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.AuthorId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ChannelHandle for the channel this message was sent in.
    /// </summary>
    public ChannelHandle? Channel()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ChannelHandle();
            ChannelHandle? __returnValue = null;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.MessageHandle.Channel(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ChannelHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the channel ID this message was sent in.
    /// </summary>
    public ulong ChannelId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.ChannelId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the content of this message, if any.
    /// </summary>
    /// <remarks>
    ///  A message can be blank if it was sent from Discord but only contains content such as image
    ///  attachments. Certain types of markup, such as markup for emojis and mentions, will be auto
    ///  replaced with a more human readable form, such as `@username` or `:emoji_name:`.
    ///
    /// </remarks>
    public string Content()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                NativeMethods.MessageHandle.Content(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  If this is an auto-generated message that is explaining some integration behavior to users,
    ///  this method will return the type of disclosure so you can customize it.
    /// </summary>
    public DisclosureTypes? DisclosureType()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            DisclosureTypes __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.MessageHandle.DisclosureType(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  The timestamp in millis since the epoch when the message was most recently edited.
    /// </summary>
    /// <remarks>
    ///  Returns 0 if the message has not been edited yet.
    ///
    /// </remarks>
    public ulong EditedTimestamp()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.EditedTimestamp(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the ID of this message.
    /// </summary>
    public ulong Id()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.Id(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns the LobbyHandle this message was sent in, if it was sent in a lobby.
    /// </summary>
    public LobbyHandle? Lobby()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.LobbyHandle();
            LobbyHandle? __returnValue = null;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.MessageHandle.Lobby(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new LobbyHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns any metadata the developer included with this message.
    /// </summary>
    /// <remarks>
    ///  Metadata is just a set of simple string key/value pairs.
    ///  An example use case might be to include a character name so you can customize how a message
    ///  renders in game.
    ///
    /// </remarks>
    public Dictionary<string, string> Metadata()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            var __returnValueNative = new NativeMethods.Discord_Properties();
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                NativeMethods.MessageHandle.Metadata(self, &__returnValueNative);
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
    ///  Returns the content of this message, if any, but without replacing any markup from emojis
    ///  and mentions.
    /// </summary>
    /// <remarks>
    ///  A message can be blank if it was sent from Discord but only contains content such as image
    ///  attachments.
    ///
    /// </remarks>
    public string RawContent()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                NativeMethods.MessageHandle.RawContent(self, &__returnValue);
            }
            string __returnValueSurface =
              Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    /// <summary>
    ///  Returns the UserHandle for the other participant in a DM, if this message was sent in a DM.
    /// </summary>
    public UserHandle? Recipient()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.UserHandle();
            UserHandle? __returnValue = null;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnIsNonNull =
                  NativeMethods.MessageHandle.Recipient(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new UserHandle(__returnValueNative, 0);
            return __returnValue;
        }
    }
    /// <summary>
    ///  When this message was sent in a DM or Ephemeral DM, this method will return the ID of the
    ///  other user in that DM.
    /// </summary>
    public ulong RecipientId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.RecipientId(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  Returns true if this message was sent in-game, otherwise false (i.e. from Discord itself).
    /// </summary>
    public bool SentFromGame()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.SentFromGame(self);
            }
            return __returnValue;
        }
    }
    /// <summary>
    ///  The timestamp in millis since the epoch when the message was sent.
    /// </summary>
    public ulong SentTimestamp()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(MessageHandle));
        }
        unsafe
        {
            ulong __returnValue;
            fixed (NativeMethods.MessageHandle* self = &this.self)
            {
                __returnValue = NativeMethods.MessageHandle.SentTimestamp(self);
            }
            return __returnValue;
        }
    }
}
