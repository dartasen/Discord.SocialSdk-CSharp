using System.Runtime.InteropServices;

namespace Discord.SocialSdk;

/// <summary>
///  An Activity represents one "thing" a user is doing on Discord and is part of their rich
///  presence.
/// </summary>
/// <remarks>
///  Additional information is located on the Discord Developer Portal:
///  - https://discord.com/developers/docs/rich-presence/overview
///  - https://discord.com/developers/docs/developer-tools/game-sdk#activities
///  - https://discord.com/developers/docs/rich-presence/best-practices
///
///  While RichPresence supports multiple types of activities, the only activity type that is really
///  relevant for the SDK is ActivityTypes::Playing. Additionally, the SDK will only expose
///  Activities that are associated with the current game (or application). So for example, a field
///  like `name` below, will always be set to the current game's name from the view of the SDK.
///
///  ## Customization
///  When an activity shows up on Discord, it will look something like this:
///  1. Playing "game name"
///  2. Capture the flag | 2 - 1
///  3. In a group (2 of 3)
///
///  You can control how lines 2 and 3 are rendered in Discord, here's the breakdown:
///  - Line 1, `Playing "game name"` is powered by the name of your game (or application) on
///  Discord.
///  - Line 2, `Capture the flag | 2 - 1` is powered by the `details` field in the activity, and
///  this should generally try to describe what the _player_ is currently doing. You can even
///  include dynamic data such as a match score here.
///  - Line 3, `In a group (2 of 3)` describes the _party_ the player is in. "Party" is used to
///  refer to a group of players in a shared context, such as a lobby, server, team, etc. The first
///  half, `In a group` is powered by the `state` field in the activity, and the second half, `(2 of
///  3)` is powered by the `party` field in the activity and describes how many people are in the
///  current party and how big the party can get.
///
///  This diagram visually shows the field mapping:
///
///
///  \image html "rich_presence.png" "Rich presence field diagram" width=1070px
///
///  ## Invites / Joinable Activities
///  Other users can be invited to join the current player's activity (or request to join it too),
///  but that does require certain fields to be set:
///  1. ActivityParty must be set and have a non-empty ActivityParty::Id field. All users in the
///  party should set the same id field too!
///  2. ActivityParty must specify the size of the group, and there must be room in the group for
///  another person to join.
///  3. ActivitySecrets::Join must be set to a non-empty string. The join secret is only shared with
///  users who are accepted into the party by an existing member, so it is truly a secret. You can
///  use this so that when the user is accepted your game knows how to join them to the party. For
///  example it could be an internal game ID, or a Discord lobby ID/secret that the client could
///  join.
///
///  There is additional information about game invites here:
///  https://support.discord.com/hc/en-us/articles/115001557452-Game-Invites
///
///  ### Mobile Invites
///  Activity invites are handled via a deep link. To enable users to join your game via an invite
///  in the Discord client, you must do two things:
///  1. Set your deep link URL in the Discord developer portal. This will be available on the
///  General tab of your application once Social Layer integration is enabled for your app.
///  2. Set the desired supported platforms when reporting the activity info in your rich presence,
///  e.g.:
///
///
///  \code
///      activity.SetSupportedPlatforms(
///          ActivityGamePlatforms.Desktop |
///          ActivityGamePlatforms.IOS |
///          ActivityGamePlatforms.Android);
///  \endcode
///
///
///  When the user accepts the invite, the Discord client will open:
///  `[your url]/_discord/join?secret=[the join secret you set]`
///
///  ### Example Invites Flow
///  If you are using Discord lobbies for your game, a neat flow would look like this:
///  - When a user starts playing the game, they create a lobby with a random secret string, using
///  Client::CreateOrJoinLobby
///  - That user publishes their RichPresence with the join secret set to the lobby secret, along
///  with party size information
///  - Another use can then see that RichPresence on Discord and join off of it
///  - Once accepted the new user receives the join secret and their client can call
///  CreateOrJoinLobby(joinSecret) to join the lobby
///  - Finally the original user can notice that the lobby membership has changed and so they
///  publish a new RichPresence update containing the updating party size information.
///
///  ### Invites Code Example
///
///  \code
///  // User A
///  // 1. Create a lobby with secret
///  std::string lobbySecret = "foo";
///  client->CreateOrJoinLobby(lobbySecret, [=](discordpp::ClientResult result, uint64_t lobbyId) {
///      // 2. Update rich presence with join secret
///      discordpp::Activity activity{};
///      // set name, state, party size ...
///      discordpp::ActivitySecrets secrets{};
///      secrets.SetJoin(lobbySecret);
///      activity.SetSecrets(secrets);
///      client->UpdateRichPresence(std::move(activity), [](discordpp::ClientResult result) {});
///  });
///  // 3. Some time later, send an invite
///  client->SendActivityInvite(USER_B_ID, "come play with me", [](auto result) {});
///
///  // User B
///  // 4. Monitor for new invites
///  client->SetActivityInviteCallback([client](auto invite) {
///      // 5. When an invite is received, ask the user if they want to accept it.
///      // If they choose to do so then go ahead and invoke AcceptActivityInvite
///      client->AcceptActivityInvite(invite,
///          [client](discordpp::ClientResult result, std::string secret) {
///          if (result.Successful()) {
///              // 5. Join the lobby using the joinSecret
///              client->CreateOrJoinLobby(secret, [](discordpp::ClientResult result, uint64_t
///              lobbyId) {
///                  // Successfully joined lobby!
///              });
///          }
///      });
///  });
///  \endcode
///
///
///  ### Join Requests Code Example
///  Users can also request to join each others parties. This code snippet shows how that flow might
///  look:
///
///  \code
///  // User A
///  // 1. Create a lobby with secret
///  std::string lobbySecret = "foo";
///  client->CreateOrJoinLobby(lobbySecret, [=](discordpp::ClientResult result, uint64_t lobbyId) {
///      // 2. Update rich presence with join secret
///      discordpp::Activity activity{};
///      // set name, state, party size ...
///      discordpp::ActivitySecrets secrets{};
///      secrets.SetJoin(lobbySecret);
///      activity.SetSecrets(secrets);
///      client->UpdateRichPresence(std::move(activity), [](discordpp::ClientResult result) {});
///  });
///
///  // User B
///  // 3. Request to join User A's party
///  client->SendActivityJoinRequest(USER_A_ID, [](auto result) {});
///
///  // User A
///  // Monitor for new invites:
///  client->SetActivityInviteCreatedCallback([client](auto invite) {
///      // 5. The game can now show that User A has received a request to join their party
///      // If User A is ok with that, they can reply back:
///      // Note: invite.type will be ActivityActionTypes::JoinRequest in this example
///      client->SendActivityJoinRequestReply(invite, [](auto result) {});
///  });
///
///  // User B
///  // 6. Same as before, user B can monitor for invites
///  client->SetActivityInviteCreatedCallback([client](auto invite) {
///      // 7. When an invite is received, ask the user if they want to accept it.
///      // If they choose to do so then go ahead and invoke AcceptActivityInvite
///      client->AcceptActivityInvite(invite,
///          [client](discordpp::ClientResult result, std::string secret) {
///          if (result.Successful()) {
///              // 5. Join the lobby using the joinSecret
///              client->CreateOrJoinLobby(secret, [](auto result, uint64_t lobbyId) {
///                  // Successfully joined lobby!
///              });
///          }
///      });
///  });
///  \endcode
///
///
/// </remarks>
public class Activity : IDisposable
{
    internal NativeMethods.Activity self;
    private int disposed_;

    internal Activity(NativeMethods.Activity self, int disposed)
    {
        this.self = self;
        disposed_ = disposed;
    }

    ~Activity() { Dispose(); }

    public Activity()
    {
        NativeMethods.__Init();
        unsafe
        {
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.Init(self);
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
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.Drop(self);
            }
        }
    }

    public Activity(Activity other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        if (other.disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(other));
        }
        unsafe
        {
            fixed (NativeMethods.Activity* otherPtr = &other.self)
            {
                fixed (NativeMethods.Activity* selfPtr = &self)
                {
                    NativeMethods.Activity.Clone(selfPtr, otherPtr);
                }
            }
        }
    }
    internal unsafe Activity(NativeMethods.Activity* otherPtr)
    {
        unsafe
        {
            fixed (NativeMethods.Activity* selfPtr = &self)
            {
                NativeMethods.Activity.Clone(selfPtr, otherPtr);
            }
        }
    }
    /// <summary>
    ///  Compares each field of the Activity struct for equality.
    /// </summary>
    public bool Equals(Activity other)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnValue;
            fixed (NativeMethods.Activity* __otherFixed = &other.self)
            {
                fixed (NativeMethods.Activity* self = &this.self)
                {
                    __returnValue = NativeMethods.Activity.Equals(self, __otherFixed);
                }
            }
            return __returnValue;
        }
    }
    public string Name()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.Name(self, &__returnValue);
            }
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetName(string value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned =
              NativeMethods.__InitStringLocal(__scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetName(self, __valueSpan);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public ActivityTypes Type()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            ActivityTypes __returnValue;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnValue = NativeMethods.Activity.Type(self);
            }
            return __returnValue;
        }
    }
    public void SetType(ActivityTypes value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetType(self, value);
            }
        }
    }
    public string? State()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.State(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetState(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetState(self, value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public string? Details()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValue = new NativeMethods.Discord_String();
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.Details(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
#if NETSTANDARD2_0
            string __returnValueSurface = MarshalP.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#else
            string __returnValueSurface = Marshal.PtrToStringUTF8((IntPtr)__returnValue.ptr, (int)__returnValue.size);
#endif
            NativeMethods.Discord_Free(__returnValue.ptr);
            return __returnValueSurface;
        }
    }
    public void SetDetails(string? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __scratch = stackalloc byte[1024];
            var __scratchUsed = 0;
            NativeMethods.Discord_String __valueSpan;
            var __valueOwned = NativeMethods.__InitNullableStringLocal(
              __scratch, &__scratchUsed, 1024, &__valueSpan, value);
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetDetails(self, value != null ? &__valueSpan : null);
            }
            NativeMethods.__FreeLocal(&__valueSpan, __valueOwned);
        }
    }
    public ulong? ApplicationId()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            ulong __returnValue;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.ApplicationId(self, &__returnValue);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            return __returnValue;
        }
    }
    public void SetApplicationId(ulong? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __valueLocal = value ?? default;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetApplicationId(self,
                                                        value != null ? &__valueLocal : null);
            }
        }
    }
    public ActivityAssets? Assets()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ActivityAssets();
            ActivityAssets? __returnValue = null;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.Assets(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ActivityAssets(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetAssets(ActivityAssets? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __valueLocal = value?.self ?? default;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetAssets(self, value != null ? &__valueLocal : null);
            }
            if (value != null)
            {
                value.self = __valueLocal;
            }
        }
    }
    public ActivityTimestamps? Timestamps()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ActivityTimestamps();
            ActivityTimestamps? __returnValue = null;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.Timestamps(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ActivityTimestamps(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetTimestamps(ActivityTimestamps? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __valueLocal = value?.self ?? default;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetTimestamps(self, value != null ? &__valueLocal : null);
            }
            if (value != null)
            {
                value.self = __valueLocal;
            }
        }
    }
    public ActivityParty? Party()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ActivityParty();
            ActivityParty? __returnValue = null;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.Party(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ActivityParty(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetParty(ActivityParty? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __valueLocal = value?.self ?? default;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetParty(self, value != null ? &__valueLocal : null);
            }
            if (value != null)
            {
                value.self = __valueLocal;
            }
        }
    }
    public ActivitySecrets? Secrets()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            bool __returnIsNonNull;
            var __returnValueNative = new NativeMethods.ActivitySecrets();
            ActivitySecrets? __returnValue = null;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnIsNonNull = NativeMethods.Activity.Secrets(self, &__returnValueNative);
            }
            if (!__returnIsNonNull)
            {
                return null;
            }
            __returnValue = new ActivitySecrets(__returnValueNative, 0);
            return __returnValue;
        }
    }
    public void SetSecrets(ActivitySecrets? value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            var __valueLocal = value?.self ?? default;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetSecrets(self, value != null ? &__valueLocal : null);
            }
            if (value != null)
            {
                value.self = __valueLocal;
            }
        }
    }
    public ActivityGamePlatforms SupportedPlatforms()
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            ActivityGamePlatforms __returnValue;
            fixed (NativeMethods.Activity* self = &this.self)
            {
                __returnValue = NativeMethods.Activity.SupportedPlatforms(self);
            }
            return __returnValue;
        }
    }
    public void SetSupportedPlatforms(ActivityGamePlatforms value)
    {
        if (disposed_ != 0)
        {
            throw new ObjectDisposedException(nameof(Activity));
        }
        unsafe
        {
            fixed (NativeMethods.Activity* self = &this.self)
            {
                NativeMethods.Activity.SetSupportedPlatforms(self, value);
            }
        }
    }
}
