using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using Discord.SocialSdk.Attributes;

namespace Discord.SocialSdk;

public static unsafe class NativeMethods
{
#if UNITY_IOS && !UNITY_EDITOR
    public const string LibraryName = "__Internal";
#else
    public const string LibraryName = "discord_partner_sdk";
#endif

    static NativeMethods()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject activity =
      unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
    AndroidJavaClass socialSdkClass =
      new AndroidJavaClass("com.discord.socialsdk.DiscordSocialSdkInit");
    socialSdkClass.CallStatic("setEngineActivity", activity);
#endif
        unsafe
        {
            // It's possible that the scripting domain was unloaded while there
            // are still pending callbacks. Reset the queue just in case.
            Discord_ResetCallbacks();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void __Init()
    {
        // The real logic is in the static constructor.
    }

    public static void __ReportUnhandledException(Exception ex)
    {
        Action<Exception>? handler = UnhandledException;
        if (handler != null)
        {
            handler(ex);
        }
        else
        {
            Console.WriteLine(ex);
        }
    }

    public delegate void Discord_FreeFn(void* ptr);

    internal class ManagedUserData
    {
        public Delegate managedCallback;

        public static void* Free;

        public ManagedUserData(Delegate managedCallback) { this.managedCallback = managedCallback; }

        static ManagedUserData()
        {
            Free = (void*)Marshal.GetFunctionPointerForDelegate<Discord_FreeFn>(UnmanagedFree);
        }

        [MonoPInvokeCallback(typeof(Discord_FreeFn))]
        public static void UnmanagedFree(void* userData)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            handle.Free();
        }

        public static T DelegateFromPointer<T>(void* userData)
          where T : Delegate
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            var userDataObj = (ManagedUserData)handle.Target!;
            return (T)userDataObj.managedCallback;
        }

        public static void* CreateHandle(Delegate cb)
        {
            var userData = new ManagedUserData(cb);
            return GCHandle.ToIntPtr(GCHandle.Alloc(userData)).ToPointer();
        }
    }

    public static event Action<Exception>? UnhandledException;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void __InitString(Discord_String* str, string value)
    {
        str->ptr = (byte*)Marshal.StringToCoTaskMemUTF8(value);
        str->size = (UIntPtr)Encoding.UTF8.GetByteCount(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void __FreeString(Discord_String* str)
    {
        Marshal.FreeCoTaskMem((IntPtr)str->ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool __InitStringLocal(byte* buf,
                                                int* bufUsed,
                                                int bufCapacity,
                                                Discord_String* str,
                                                string value)
    {
        var byteCount = Encoding.UTF8.GetByteCount(value);
        if (*bufUsed + byteCount > bufCapacity)
        {
            str->ptr = (byte*)Marshal.StringToCoTaskMemUTF8(value);
            str->size = (UIntPtr)byteCount;
            return true;
        }
        var span = new Span<byte>(buf + *bufUsed, bufCapacity - *bufUsed);
        var byteCountWritten = Encoding.UTF8.GetBytes(value, span);
        str->ptr = buf + *bufUsed;
        *bufUsed += byteCountWritten;
        str->size = (UIntPtr)byteCount;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool __InitNullableStringLocal(byte* buf,
                                                        int* bufUsed,
                                                        int bufCapacity,
                                                        Discord_String* str,
                                                        string? value)
    {
        if (value == null)
        {
            str->ptr = null;
            str->size = UIntPtr.Zero;
            return false;
        }
        return __InitStringLocal(buf, bufUsed, bufCapacity, str, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool __AllocLocal(byte* buf,
                                           int* bufUsed,
                                           int bufCapacity,
                                           void** ptrOut,
                                           int size)
    {
        var alignedSize = size + 7 & ~7;
        if (*bufUsed + alignedSize > bufCapacity)
        {
            *ptrOut = (void*)Marshal.AllocCoTaskMem(size);
            return true;
        }
        *ptrOut = buf + *bufUsed + (alignedSize - size);
        *bufUsed += alignedSize;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool __AllocLocalStringArray(byte* buf,
                                                      int* bufUsed,
                                                      int bufCapacity,
                                                      Discord_String** ptrOut,
                                                      int count)
    {
        void* ptr;
        var owned = __AllocLocal(
          buf, bufUsed, bufCapacity, &ptr, count * sizeof(Discord_String));
        *ptrOut = (Discord_String*)ptr;
        return owned;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool __AllocateLocalBoolArray(byte* buf,
                                                       int* bufUsed,
                                                       int bufCapacity,
                                                       bool** ptrOut,
                                                       int count)
    {
        void* ptr;
        var owned = __AllocLocal(buf, bufUsed, bufCapacity, &ptr, count * sizeof(bool));
        *ptrOut = (bool*)ptr;
        return owned;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void __FreeLocal(Discord_String* str, bool owned)
    {
        if (owned)
        {
            Marshal.FreeCoTaskMem((IntPtr)str->ptr);
        }
    }

    [DllImport(LibraryName,
               EntryPoint = "Discord_Alloc",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void* Discord_Alloc(UIntPtr size);

    [DllImport(LibraryName,
               EntryPoint = "Discord_Free",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void Discord_Free(void* ptr);

    [DllImport(LibraryName,
               EntryPoint = "Discord_FreeProperties",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void Discord_FreeProperties(Discord_Properties props);

    [DllImport(LibraryName,
               EntryPoint = "Discord_SetFreeThreaded",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void Discord_SetFreeThreaded();

    [DllImport(LibraryName,
               EntryPoint = "Discord_RunCallbacks",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void Discord_RunCallbacks();

    [DllImport(LibraryName,
               EntryPoint = "Discord_ResetCallbacks",
               CallingConvention = CallingConvention.Cdecl)]
    public static extern void Discord_ResetCallbacks();

    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_String
    {
        public byte* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_UInt64Span
    {
        public ulong* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_LobbyMemberHandleSpan
    {
        public LobbyMemberHandle* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_CallSpan
    {
        public Call* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_AudioDeviceSpan
    {
        public AudioDevice* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_GuildChannelSpan
    {
        public GuildChannel* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_GuildMinimalSpan
    {
        public GuildMinimal* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_RelationshipHandleSpan
    {
        public RelationshipHandle* ptr;
        public UIntPtr size;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Discord_UserHandleSpan
    {
        public UserHandle* ptr;
        public UIntPtr size;
    }
    public struct Discord_Properties
    {
        public IntPtr size;
        public Discord_String* keys;
        public Discord_String* values;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivityInvite
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ActivityInvite* self, ActivityInvite* rhs);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SenderId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong SenderId(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetSenderId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSenderId(ActivityInvite* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_ChannelId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelId(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetChannelId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetChannelId(ActivityInvite* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_MessageId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong MessageId(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetMessageId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMessageId(ActivityInvite* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_Type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ActivityActionTypes Type(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetType(ActivityInvite* self,
                                          ActivityActionTypes value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_ApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ApplicationId(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetApplicationId(ActivityInvite* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_PartyId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void PartyId(ActivityInvite* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetPartyId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetPartyId(ActivityInvite* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SessionId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SessionId(ActivityInvite* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetSessionId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSessionId(ActivityInvite* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_IsValid",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsValid(ActivityInvite* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityInvite_SetIsValid",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIsValid(ActivityInvite* self, bool value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivityAssets
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ActivityAssets* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ActivityAssets* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ActivityAssets* self, ActivityAssets* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_LargeImage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LargeImage(ActivityAssets* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SetLargeImage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLargeImage(ActivityAssets* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_LargeText",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LargeText(ActivityAssets* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SetLargeText",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLargeText(ActivityAssets* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SmallImage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SmallImage(ActivityAssets* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SetSmallImage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSmallImage(ActivityAssets* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SmallText",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SmallText(ActivityAssets* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityAssets_SetSmallText",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSmallText(ActivityAssets* self, Discord_String* value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivityTimestamps
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ActivityTimestamps* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ActivityTimestamps* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ActivityTimestamps* self, ActivityTimestamps* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_Start",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Start(ActivityTimestamps* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_SetStart",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStart(ActivityTimestamps* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_End",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong End(ActivityTimestamps* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityTimestamps_SetEnd",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetEnd(ActivityTimestamps* self, ulong value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivityParty
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ActivityParty* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ActivityParty* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ActivityParty* self, ActivityParty* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Id(ActivityParty* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_SetId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetId(ActivityParty* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_CurrentSize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurrentSize(ActivityParty* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_SetCurrentSize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCurrentSize(ActivityParty* self, int value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_MaxSize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int MaxSize(ActivityParty* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_SetMaxSize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMaxSize(ActivityParty* self, int value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_Privacy",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ActivityPartyPrivacy Privacy(ActivityParty* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivityParty_SetPrivacy",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetPrivacy(ActivityParty* self,
                                             ActivityPartyPrivacy value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivitySecrets
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivitySecrets_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ActivitySecrets* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivitySecrets_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ActivitySecrets* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivitySecrets_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ActivitySecrets* self, ActivitySecrets* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivitySecrets_Join",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Join(ActivitySecrets* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ActivitySecrets_SetJoin",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetJoin(ActivitySecrets* self, Discord_String value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Activity
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(Activity* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(Activity* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(Activity* self, Activity* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Equals",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Equals(Activity* self, Activity* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(Activity* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetName(Activity* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ActivityTypes Type(Activity* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetType(Activity* self, ActivityTypes value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_State",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool State(Activity* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetState",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetState(Activity* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Details",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Details(Activity* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetDetails",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDetails(Activity* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_ApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ApplicationId(Activity* self, ulong* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetApplicationId(Activity* self, ulong* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Assets",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Assets(Activity* self, ActivityAssets* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetAssets",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAssets(Activity* self, ActivityAssets* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Timestamps",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Timestamps(Activity* self, ActivityTimestamps* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetTimestamps",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTimestamps(Activity* self, ActivityTimestamps* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Party",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Party(Activity* self, ActivityParty* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetParty",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetParty(Activity* self, ActivityParty* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_Secrets",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Secrets(Activity* self, ActivitySecrets* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetSecrets",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSecrets(Activity* self, ActivitySecrets* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SupportedPlatforms",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ActivityGamePlatforms SupportedPlatforms(Activity* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Activity_SetSupportedPlatforms",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSupportedPlatforms(Activity* self,
                                                        ActivityGamePlatforms value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ClientResult
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ClientResult* self, ClientResult* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_ToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ToString(ClientResult* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ErrorType Type(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetType(ClientResult* self, ErrorType value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Error",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Error(ClientResult* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetError",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetError(ClientResult* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_ErrorCode",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int ErrorCode(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetErrorCode",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetErrorCode(ClientResult* self, int value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Status",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern HttpStatusCode Status(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetStatus",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStatus(ClientResult* self, HttpStatusCode value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_ResponseBody",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResponseBody(ClientResult* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetResponseBody",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetResponseBody(ClientResult* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Successful",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Successful(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetSuccessful",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSuccessful(ClientResult* self, bool value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_Retryable",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Retryable(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetRetryable",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRetryable(ClientResult* self, bool value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_RetryAfter",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float RetryAfter(ClientResult* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ClientResult_SetRetryAfter",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRetryAfter(ClientResult* self, float value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthorizationCodeChallenge
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(AuthorizationCodeChallenge* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(AuthorizationCodeChallenge* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(AuthorizationCodeChallenge* self,
                                        AuthorizationCodeChallenge* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_Method",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern AuthenticationCodeChallengeMethod Method(
          AuthorizationCodeChallenge* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_SetMethod",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMethod(AuthorizationCodeChallenge* self,
                                            AuthenticationCodeChallengeMethod value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_Challenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Challenge(AuthorizationCodeChallenge* self,
                                            Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeChallenge_SetChallenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetChallenge(AuthorizationCodeChallenge* self,
                                               Discord_String value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthorizationCodeVerifier
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(AuthorizationCodeVerifier* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(AuthorizationCodeVerifier* self,
                                        AuthorizationCodeVerifier* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_Challenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Challenge(AuthorizationCodeVerifier* self,
                                            AuthorizationCodeChallenge* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_SetChallenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetChallenge(AuthorizationCodeVerifier* self,
                                               AuthorizationCodeChallenge* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_Verifier",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Verifier(AuthorizationCodeVerifier* self,
                                           Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationCodeVerifier_SetVerifier",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetVerifier(AuthorizationCodeVerifier* self,
                                              Discord_String value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthorizationArgs
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(AuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(AuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(AuthorizationArgs* self, AuthorizationArgs* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_ClientId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ClientId(AuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_SetClientId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetClientId(AuthorizationArgs* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_Scopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Scopes(AuthorizationArgs* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_SetScopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetScopes(AuthorizationArgs* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_State",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool State(AuthorizationArgs* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_SetState",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetState(AuthorizationArgs* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_Nonce",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Nonce(AuthorizationArgs* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_SetNonce",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNonce(AuthorizationArgs* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_CodeChallenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CodeChallenge(AuthorizationArgs* self,
                                                AuthorizationCodeChallenge* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AuthorizationArgs_SetCodeChallenge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCodeChallenge(AuthorizationArgs* self,
                                                   AuthorizationCodeChallenge* value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceAuthorizationArgs
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(DeviceAuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(DeviceAuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(DeviceAuthorizationArgs* self,
                                        DeviceAuthorizationArgs* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_ClientId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ClientId(DeviceAuthorizationArgs* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_SetClientId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetClientId(DeviceAuthorizationArgs* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_Scopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Scopes(DeviceAuthorizationArgs* self,
                                         Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_DeviceAuthorizationArgs_SetScopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetScopes(DeviceAuthorizationArgs* self, Discord_String value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VoiceStateHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VoiceStateHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(VoiceStateHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VoiceStateHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(VoiceStateHandle* self, VoiceStateHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VoiceStateHandle_SelfDeaf",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SelfDeaf(VoiceStateHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VoiceStateHandle_SelfMute",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SelfMute(VoiceStateHandle* self);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VADThresholdSettings
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VADThresholdSettings_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(VADThresholdSettings* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VADThresholdSettings_VadThreshold",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float VadThreshold(VADThresholdSettings* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VADThresholdSettings_SetVadThreshold",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetVadThreshold(VADThresholdSettings* self, float value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VADThresholdSettings_Automatic",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Automatic(VADThresholdSettings* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_VADThresholdSettings_SetAutomatic",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAutomatic(VADThresholdSettings* self, bool value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Call
    {
        public IntPtr Opaque0;
        public IntPtr Opaque1;
        public IntPtr Opaque2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnVoiceStateChanged(ulong userId, void* __userData);

        [MonoPInvokeCallback(typeof(OnVoiceStateChanged))]
        public static void OnVoiceStateChanged_Handler(ulong userId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Call.OnVoiceStateChanged>(__userData);
            try
            {
                __callback(userId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnParticipantChanged(ulong userId, bool added, void* __userData);


        [MonoPInvokeCallback(typeof(OnParticipantChanged))]
        public static void OnParticipantChanged_Handler(ulong userId,
                                                        bool added,
                                                        void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Call.OnParticipantChanged>(__userData);
            try
            {
                __callback(userId, added);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnSpeakingStatusChanged(ulong userId,
                                                     bool isPlayingSound,
                                                     void* __userData);
        [MonoPInvokeCallback(typeof(OnSpeakingStatusChanged))]
        public static void OnSpeakingStatusChanged_Handler(ulong userId,
                                                           bool isPlayingSound,
                                                           void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Call.OnSpeakingStatusChanged>(__userData);
            try
            {
                __callback(userId, isPlayingSound);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnStatusChanged(SocialSdk.Call.Status status,
                                             SocialSdk.Call.Error error,
                                             int errorDetail,
                                             void* __userData);
#if UNITY
        [MonoPInvokeCallback(typeof(OnStatusChanged))]
#endif
        public static void OnStatusChanged_Handler(SocialSdk.Call.Status status,
                                                   SocialSdk.Call.Error error,
                                                   int errorDetail,
                                                   void* __userData)
        {
            var __callback =
              ManagedUserData.DelegateFromPointer<SocialSdk.Call.OnStatusChanged>(
                __userData);
            try
            {
                __callback(status, error, errorDetail);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(Call* self, Call* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_ErrorToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ErrorToString(SocialSdk.Call.Error type,
                                                Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetAudioMode",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern AudioModeType GetAudioMode(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetChannelId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetChannelId(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetGuildId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetGuildId(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetLocalMute",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLocalMute(Call* self, ulong userId);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetParticipants",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetParticipants(Call* self, Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetParticipantVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetParticipantVolume(Call* self, ulong userId);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetPTTActive",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetPTTActive(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetPTTReleaseDelay",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetPTTReleaseDelay(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetSelfDeaf",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSelfDeaf(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetSelfMute",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSelfMute(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetStatus",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern SocialSdk.Call.Status GetStatus(Call* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetVADThreshold",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetVADThreshold(Call* self, VADThresholdSettings* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_GetVoiceStateHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetVoiceStateHandle(Call* self,
                                                      ulong userId,
                                                      VoiceStateHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetAudioMode",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAudioMode(Call* self, AudioModeType audioMode);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetLocalMute",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLocalMute(Call* self, ulong userId, bool mute);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetOnVoiceStateChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOnVoiceStateChangedCallback(
          Call* self,
          OnVoiceStateChanged cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetParticipantChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetParticipantChangedCallback(
          Call* self,
          OnParticipantChanged cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetParticipantVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetParticipantVolume(Call* self, ulong userId, float volume);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetPTTActive",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetPTTActive(Call* self, bool active);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetPTTReleaseDelay",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetPTTReleaseDelay(Call* self, uint releaseDelayMs);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetSelfDeaf",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSelfDeaf(Call* self, bool deaf);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetSelfMute",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSelfMute(Call* self, bool mute);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetSpeakingStatusChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSpeakingStatusChangedCallback(
          Call* self,
          OnSpeakingStatusChanged cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetStatusChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStatusChangedCallback(
          Call* self,
          OnStatusChanged cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_SetVADThreshold",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetVADThreshold(Call* self, bool automatic, float threshold);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Call_StatusToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatusToString(SocialSdk.Call.Status type,
                                                 Discord_String* returnValue);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ChannelHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(ChannelHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(ChannelHandle* self, ChannelHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(ChannelHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(ChannelHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Recipients",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Recipients(ChannelHandle* self, Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_ChannelHandle_Type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ChannelType Type(ChannelHandle* self);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct GuildMinimal
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(GuildMinimal* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(GuildMinimal* self, GuildMinimal* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(GuildMinimal* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_SetId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetId(GuildMinimal* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(GuildMinimal* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildMinimal_SetName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetName(GuildMinimal* self, Discord_String value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct GuildChannel
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(GuildChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(GuildChannel* self, GuildChannel* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(GuildChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_SetId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetId(GuildChannel* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(GuildChannel* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_SetName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetName(GuildChannel* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_IsLinkable",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsLinkable(GuildChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_SetIsLinkable",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIsLinkable(GuildChannel* self, bool value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_IsViewableAndWriteableByAllMembers",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsViewableAndWriteableByAllMembers(GuildChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_SetIsViewableAndWriteableByAllMembers",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIsViewableAndWriteableByAllMembers(GuildChannel* self,
                                                                        bool value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_LinkedLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LinkedLobby(GuildChannel* self, LinkedLobby* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_GuildChannel_SetLinkedLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLinkedLobby(GuildChannel* self, LinkedLobby* value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct LinkedLobby
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(LinkedLobby* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(LinkedLobby* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(LinkedLobby* self, LinkedLobby* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_ApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ApplicationId(LinkedLobby* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_SetApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetApplicationId(LinkedLobby* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_LobbyId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong LobbyId(LinkedLobby* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedLobby_SetLobbyId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyId(LinkedLobby* self, ulong value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct LinkedChannel
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(LinkedChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(LinkedChannel* self, LinkedChannel* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(LinkedChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_SetId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetId(LinkedChannel* self, ulong value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(LinkedChannel* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_SetName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetName(LinkedChannel* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_GuildId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GuildId(LinkedChannel* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LinkedChannel_SetGuildId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGuildId(LinkedChannel* self, ulong value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RelationshipHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(RelationshipHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(RelationshipHandle* self, RelationshipHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_DiscordRelationshipType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern RelationshipType DiscordRelationshipType(
          RelationshipHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_GameRelationshipType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern RelationshipType GameRelationshipType(
          RelationshipHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(RelationshipHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_RelationshipHandle_User",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool User(RelationshipHandle* self, UserHandle* returnValue);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct UserHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(UserHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(UserHandle* self, UserHandle* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Avatar",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Avatar(UserHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_AvatarTypeToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvatarTypeToString(SocialSdk.UserHandle.AvatarType type,
                                                     Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_AvatarUrl",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AvatarUrl(UserHandle* self,
                                            SocialSdk.UserHandle.AvatarType animatedType,
                                            SocialSdk.UserHandle.AvatarType staticType,
                                            Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_DisplayName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisplayName(UserHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_GameActivity",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GameActivity(UserHandle* self, Activity* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_GlobalName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GlobalName(UserHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(UserHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_IsProvisional",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsProvisional(UserHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Relationship",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Relationship(UserHandle* self, RelationshipHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Status",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern StatusType Status(UserHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_UserHandle_Username",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Username(UserHandle* self, Discord_String* returnValue);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct LobbyMemberHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(LobbyMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(LobbyMemberHandle* self, LobbyMemberHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_CanLinkLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CanLinkLobby(LobbyMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_Connected",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Connected(LobbyMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(LobbyMemberHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_Metadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Metadata(
          LobbyMemberHandle* self,
          Discord_Properties* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyMemberHandle_User",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool User(LobbyMemberHandle* self, UserHandle* returnValue);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct LobbyHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(LobbyHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(LobbyHandle* self, LobbyHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_GetCallInfoHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetCallInfoHandle(LobbyHandle* self, CallInfoHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_GetLobbyMemberHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLobbyMemberHandle(LobbyHandle* self,
                                                       ulong memberId,
                                                       LobbyMemberHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(LobbyHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_LinkedChannel",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LinkedChannel(LobbyHandle* self, LinkedChannel* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_LobbyMemberIds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void LobbyMemberIds(LobbyHandle* self,
                                                 Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_LobbyMembers",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void LobbyMembers(LobbyHandle* self,
                                               Discord_LobbyMemberHandleSpan* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_LobbyHandle_Metadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Metadata(
          LobbyHandle* self,
          Discord_Properties* returnValue);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AdditionalContent
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(AdditionalContent* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(AdditionalContent* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(AdditionalContent* self, AdditionalContent* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Equals",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Equals(AdditionalContent* self, AdditionalContent* rhs);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_TypeToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void TypeToString(AdditionalContentType type,
                                               Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern AdditionalContentType Type(AdditionalContent* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_SetType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetType(AdditionalContent* self,
                                          AdditionalContentType value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Title",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Title(AdditionalContent* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_SetTitle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTitle(AdditionalContent* self, Discord_String* value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_Count",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern byte Count(AdditionalContent* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AdditionalContent_SetCount",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCount(AdditionalContent* self, byte value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MessageHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(MessageHandle* self, MessageHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_AdditionalContent",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AdditionalContent(MessageHandle* self,
                                                    AdditionalContent* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Author",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Author(MessageHandle* self, UserHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_AuthorId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong AuthorId(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Channel",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Channel(MessageHandle* self, ChannelHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_ChannelId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelId(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Content",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Content(MessageHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_DisclosureType",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DisclosureType(MessageHandle* self,
                                                 DisclosureTypes* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_EditedTimestamp",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong EditedTimestamp(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong Id(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Lobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Lobby(MessageHandle* self, LobbyHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Metadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Metadata(
          MessageHandle* self,
          Discord_Properties* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_RawContent",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RawContent(MessageHandle* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_Recipient",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Recipient(MessageHandle* self, UserHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_RecipientId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong RecipientId(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_SentFromGame",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SentFromGame(MessageHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_MessageHandle_SentTimestamp",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong SentTimestamp(MessageHandle* self);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioDevice
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(AudioDevice* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(AudioDevice* self, AudioDevice* arg0);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_Equals",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Equals(AudioDevice* self, AudioDevice* rhs);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_Id",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Id(AudioDevice* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_SetId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetId(AudioDevice* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_Name",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Name(AudioDevice* self, Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_SetName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetName(AudioDevice* self, Discord_String value);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_IsDefault",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsDefault(AudioDevice* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_AudioDevice_SetIsDefault",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIsDefault(AudioDevice* self, bool value);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Client
    {
        public IntPtr Handle;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EndCallCallback(void* __userData);
        [MonoPInvokeCallback(typeof(EndCallCallback))]
        public static void EndCallCallback_Handler(void* __userData)
        {
            var __callback =
              ManagedUserData.DelegateFromPointer<SocialSdk.Client.EndCallCallback>(
                __userData);
            try
            {
                __callback();
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EndCallsCallback(void* __userData);
        [MonoPInvokeCallback(typeof(EndCallsCallback))]
        public static void EndCallsCallback_Handler(void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.EndCallsCallback>(__userData);
            try
            {
                __callback();
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetCurrentInputDeviceCallback(AudioDevice* device, void* __userData);
        [MonoPInvokeCallback(typeof(GetCurrentInputDeviceCallback))]
        public static void GetCurrentInputDeviceCallback_Handler(AudioDevice* device,
                                                                 void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetCurrentInputDeviceCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.AudioDevice(device));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetCurrentOutputDeviceCallback(AudioDevice* device, void* __userData);
        [MonoPInvokeCallback(typeof(GetCurrentOutputDeviceCallback))]
        public static void GetCurrentOutputDeviceCallback_Handler(AudioDevice* device,
                                                                  void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetCurrentOutputDeviceCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.AudioDevice(device));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetInputDevicesCallback(Discord_AudioDeviceSpan devices,
                                                     void* __userData);
        [MonoPInvokeCallback(typeof(GetInputDevicesCallback))]
        public static void GetInputDevicesCallback_Handler(Discord_AudioDeviceSpan devices,
                                                           void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetInputDevicesCallback>(__userData);
            try
            {
                __callback(new Span<AudioDevice>(devices.ptr, (int)devices.size)
                             .ToArray()
                             .Select(__native => new Discord.SocialSdk.AudioDevice(__native, 0))
                             .ToArray());
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(devices.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetOutputDevicesCallback(Discord_AudioDeviceSpan devices,
                                                      void* __userData);
        [MonoPInvokeCallback(typeof(GetOutputDevicesCallback))]
        public static void GetOutputDevicesCallback_Handler(Discord_AudioDeviceSpan devices,
                                                            void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetOutputDevicesCallback>(__userData);
            try
            {
                __callback(new Span<AudioDevice>(devices.ptr, (int)devices.size)
                             .ToArray()
                             .Select(__native => new Discord.SocialSdk.AudioDevice(__native, 0))
                             .ToArray());
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(devices.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DeviceChangeCallback(Discord_AudioDeviceSpan inputDevices,
                                                  Discord_AudioDeviceSpan outputDevices,
                                                  void* __userData);
        [MonoPInvokeCallback(typeof(DeviceChangeCallback))]
        public static void DeviceChangeCallback_Handler(Discord_AudioDeviceSpan inputDevices,
                                                        Discord_AudioDeviceSpan outputDevices,
                                                        void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.DeviceChangeCallback>(__userData);
            try
            {
                __callback(
                  new Span<AudioDevice>(inputDevices.ptr, (int)inputDevices.size)
                    .ToArray()
                    .Select(__native => new Discord.SocialSdk.AudioDevice(__native, 0))
                    .ToArray(),
                  new Span<AudioDevice>(outputDevices.ptr, (int)outputDevices.size)
                    .ToArray()
                    .Select(__native => new Discord.SocialSdk.AudioDevice(__native, 0))
                    .ToArray());
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(inputDevices.ptr);
                Discord_Free(outputDevices.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetInputDeviceCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(SetInputDeviceCallback))]
        public static void SetInputDeviceCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.SetInputDeviceCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NoAudioInputCallback(bool inputDetected, void* __userData);
        [MonoPInvokeCallback(typeof(NoAudioInputCallback))]
        public static void NoAudioInputCallback_Handler(bool inputDetected, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.NoAudioInputCallback>(__userData);
            try
            {
                __callback(inputDetected);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetOutputDeviceCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(SetOutputDeviceCallback))]
        public static void SetOutputDeviceCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.SetOutputDeviceCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VoiceParticipantChangedCallback(ulong lobbyId,
                                                             ulong memberId,
                                                             bool added,
                                                             void* __userData);
        [MonoPInvokeCallback(typeof(VoiceParticipantChangedCallback))]
        public static void VoiceParticipantChangedCallback_Handler(ulong lobbyId,
                                                                   ulong memberId,
                                                                   bool added,
                                                                   void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.VoiceParticipantChangedCallback>(
                  __userData);
            try
            {
                __callback(lobbyId, memberId, added);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UserAudioReceivedCallback(ulong userId,
                                                       short* data,
                                                       ulong samplesPerChannel,
                                                       int sampleRate,
                                                       ulong channels,
                                                       bool* outShouldMute,
                                                       void* __userData);
        [MonoPInvokeCallback(typeof(UserAudioReceivedCallback))]
        public static void UserAudioReceivedCallback_Handler(ulong userId,
                                                             short* data,
                                                             ulong samplesPerChannel,
                                                             int sampleRate,
                                                             ulong channels,
                                                             bool* outShouldMute,
                                                             void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UserAudioReceivedCallback>(__userData);
            try
            {
                __callback(userId,
                           (IntPtr)data,
                           samplesPerChannel,
                           sampleRate,
                           channels,
                           ref *outShouldMute);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UserAudioCapturedCallback(short* data,
                                                       ulong samplesPerChannel,
                                                       int sampleRate,
                                                       ulong channels,
                                                       void* __userData);
        [MonoPInvokeCallback(typeof(UserAudioCapturedCallback))]
        public static void UserAudioCapturedCallback_Handler(short* data,
                                                             ulong samplesPerChannel,
                                                             int sampleRate,
                                                             ulong channels,
                                                             void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UserAudioCapturedCallback>(__userData);
            try
            {
                __callback((IntPtr)data, samplesPerChannel, sampleRate, channels);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AuthorizationCallback(ClientResult* result,
                                                   Discord_String code,
                                                   Discord_String redirectUri,
                                                   void* __userData);
        [MonoPInvokeCallback(typeof(AuthorizationCallback))]
        public static void AuthorizationCallback_Handler(ClientResult* result,
                                                         Discord_String code,
                                                         Discord_String redirectUri,
                                                         void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.AuthorizationCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0),
                           Marshal.PtrToStringUTF8((IntPtr)code.ptr, (int)code.size),
                           Marshal.PtrToStringUTF8((IntPtr)redirectUri.ptr, (int)redirectUri.size));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(code.ptr);
                Discord_Free(redirectUri.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FetchCurrentUserCallback(ClientResult* result,
                                                      ulong id,
                                                      Discord_String name,
                                                      void* __userData);
        [MonoPInvokeCallback(typeof(FetchCurrentUserCallback))]
        public static void FetchCurrentUserCallback_Handler(ClientResult* result,
                                                            ulong id,
                                                            Discord_String name,
                                                            void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.FetchCurrentUserCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0),
                           id,
                           Marshal.PtrToStringUTF8((IntPtr)name.ptr, (int)name.size));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(name.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TokenExchangeCallback(ClientResult* result,
                                                   Discord_String accessToken,
                                                   Discord_String refreshToken,
                                                   AuthorizationTokenType tokenType,
                                                   int expiresIn,
                                                   Discord_String scopes,
                                                   void* __userData);
        [MonoPInvokeCallback(typeof(TokenExchangeCallback))]
        public static void TokenExchangeCallback_Handler(
          ClientResult* result,
          Discord_String accessToken,
          Discord_String refreshToken,
          AuthorizationTokenType tokenType,
          int expiresIn,
          Discord_String scopes,
          void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.TokenExchangeCallback>(__userData);
            try
            {
                __callback(
                  new Discord.SocialSdk.ClientResult(*result, 0),
                  Marshal.PtrToStringUTF8((IntPtr)accessToken.ptr, (int)accessToken.size),
                  Marshal.PtrToStringUTF8((IntPtr)refreshToken.ptr, (int)refreshToken.size),
                  tokenType,
                  expiresIn,
                  Marshal.PtrToStringUTF8((IntPtr)scopes.ptr, (int)scopes.size));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(accessToken.ptr);
                Discord_Free(refreshToken.ptr);
                Discord_Free(scopes.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AuthorizeDeviceScreenClosedCallback(void* __userData);
        [MonoPInvokeCallback(typeof(AuthorizeDeviceScreenClosedCallback))]
        public static void AuthorizeDeviceScreenClosedCallback_Handler(void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.AuthorizeDeviceScreenClosedCallback>(
                  __userData);
            try
            {
                __callback();
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TokenExpirationCallback(void* __userData);
        [MonoPInvokeCallback(typeof(TokenExpirationCallback))]
        public static void TokenExpirationCallback_Handler(void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.TokenExpirationCallback>(__userData);
            try
            {
                __callback();
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateProvisionalAccountDisplayNameCallback(ClientResult* result,
                                                                         void* __userData);
        [MonoPInvokeCallback(typeof(UpdateProvisionalAccountDisplayNameCallback))]
        public static void UpdateProvisionalAccountDisplayNameCallback_Handler(ClientResult* result,
                                                                               void* __userData)
        {
            var __callback = ManagedUserData.DelegateFromPointer<
              SocialSdk.Client.UpdateProvisionalAccountDisplayNameCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateTokenCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(UpdateTokenCallback))]
        public static void UpdateTokenCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UpdateTokenCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DeleteUserMessageCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(DeleteUserMessageCallback))]
        public static void DeleteUserMessageCallback_Handler(ClientResult* result,
                                                             void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.DeleteUserMessageCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EditUserMessageCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(EditUserMessageCallback))]
        public static void EditUserMessageCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.EditUserMessageCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ProvisionalUserMergeRequiredCallback(void* __userData);
        [MonoPInvokeCallback(typeof(ProvisionalUserMergeRequiredCallback))]
        public static void ProvisionalUserMergeRequiredCallback_Handler(void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.ProvisionalUserMergeRequiredCallback>(
                  __userData);
            try
            {
                __callback();
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OpenMessageInDiscordCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(OpenMessageInDiscordCallback))]
        public static void OpenMessageInDiscordCallback_Handler(ClientResult* result,
                                                                void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.OpenMessageInDiscordCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SendUserMessageCallback(ClientResult* result,
                                                     ulong messageId,
                                                     void* __userData);
        [MonoPInvokeCallback(typeof(SendUserMessageCallback))]
        public static void SendUserMessageCallback_Handler(ClientResult* result,
                                                           ulong messageId,
                                                           void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.SendUserMessageCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0), messageId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MessageCreatedCallback(ulong messageId, void* __userData);
        [MonoPInvokeCallback(typeof(MessageCreatedCallback))]
        public static void MessageCreatedCallback_Handler(ulong messageId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.MessageCreatedCallback>(__userData);
            try
            {
                __callback(messageId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MessageDeletedCallback(ulong messageId,
                                                    ulong channelId,
                                                    void* __userData);
        [MonoPInvokeCallback(typeof(MessageDeletedCallback))]
        public static void MessageDeletedCallback_Handler(ulong messageId,
                                                          ulong channelId,
                                                          void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.MessageDeletedCallback>(__userData);
            try
            {
                __callback(messageId, channelId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MessageUpdatedCallback(ulong messageId, void* __userData);
        [MonoPInvokeCallback(typeof(MessageUpdatedCallback))]
        public static void MessageUpdatedCallback_Handler(ulong messageId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.MessageUpdatedCallback>(__userData);
            try
            {
                __callback(messageId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallback(Discord_String message,
                                         LoggingSeverity severity,
                                         void* __userData);
        [MonoPInvokeCallback(typeof(LogCallback))]
        public static void LogCallback_Handler(Discord_String message,
                                               LoggingSeverity severity,
                                               void* __userData)
        {
            var __callback =
              ManagedUserData.DelegateFromPointer<SocialSdk.Client.LogCallback>(
                __userData);
            try
            {
                __callback(Marshal.PtrToStringUTF8((IntPtr)message.ptr, (int)message.size),
                           severity);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(message.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnStatusChanged(SocialSdk.Client.Status status,
                                             SocialSdk.Client.Error error,
                                             int errorDetail,
                                             void* __userData);
        [MonoPInvokeCallback(typeof(OnStatusChanged))]
        public static void OnStatusChanged_Handler(SocialSdk.Client.Status status,
                                                   SocialSdk.Client.Error error,
                                                   int errorDetail,
                                                   void* __userData)
        {
            var __callback =
              ManagedUserData.DelegateFromPointer<SocialSdk.Client.OnStatusChanged>(
                __userData);
            try
            {
                __callback(status, error, errorDetail);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CreateOrJoinLobbyCallback(ClientResult* result,
                                                       ulong lobbyId,
                                                       void* __userData);
        [MonoPInvokeCallback(typeof(CreateOrJoinLobbyCallback))]
        public static void CreateOrJoinLobbyCallback_Handler(ClientResult* result,
                                                             ulong lobbyId,
                                                             void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.CreateOrJoinLobbyCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0), lobbyId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetGuildChannelsCallback(ClientResult* result,
                                                      Discord_GuildChannelSpan guildChannels,
                                                      void* __userData);
        [MonoPInvokeCallback(typeof(GetGuildChannelsCallback))]
        public static void GetGuildChannelsCallback_Handler(ClientResult* result,
                                                            Discord_GuildChannelSpan guildChannels,
                                                            void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetGuildChannelsCallback>(__userData);
            try
            {
                __callback(
                  new Discord.SocialSdk.ClientResult(*result, 0),
                  new Span<GuildChannel>(guildChannels.ptr, (int)guildChannels.size)
                    .ToArray()
                    .Select(__native => new Discord.SocialSdk.GuildChannel(__native, 0))
                    .ToArray());
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(guildChannels.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetUserGuildsCallback(ClientResult* result,
                                                   Discord_GuildMinimalSpan guilds,
                                                   void* __userData);
        [MonoPInvokeCallback(typeof(GetUserGuildsCallback))]
        public static void GetUserGuildsCallback_Handler(ClientResult* result,
                                                         Discord_GuildMinimalSpan guilds,
                                                         void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetUserGuildsCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0),
                           new Span<GuildMinimal>(guilds.ptr, (int)guilds.size)
                             .ToArray()
                             .Select(__native => new Discord.SocialSdk.GuildMinimal(__native, 0))
                             .ToArray());
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(guilds.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LeaveLobbyCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(LeaveLobbyCallback))]
        public static void LeaveLobbyCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LeaveLobbyCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LinkOrUnlinkChannelCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(LinkOrUnlinkChannelCallback))]
        public static void LinkOrUnlinkChannelCallback_Handler(ClientResult* result,
                                                               void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LinkOrUnlinkChannelCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyCreatedCallback(ulong lobbyId, void* __userData);
        [MonoPInvokeCallback(typeof(LobbyCreatedCallback))]
        public static void LobbyCreatedCallback_Handler(ulong lobbyId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyCreatedCallback>(__userData);
            try
            {
                __callback(lobbyId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyDeletedCallback(ulong lobbyId, void* __userData);
        [MonoPInvokeCallback(typeof(LobbyDeletedCallback))]
        public static void LobbyDeletedCallback_Handler(ulong lobbyId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyDeletedCallback>(__userData);
            try
            {
                __callback(lobbyId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyMemberAddedCallback(ulong lobbyId,
                                                      ulong memberId,
                                                      void* __userData);
        [MonoPInvokeCallback(typeof(LobbyMemberAddedCallback))]
        public static void LobbyMemberAddedCallback_Handler(ulong lobbyId,
                                                            ulong memberId,
                                                            void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyMemberAddedCallback>(__userData);
            try
            {
                __callback(lobbyId, memberId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyMemberRemovedCallback(ulong lobbyId,
                                                        ulong memberId,
                                                        void* __userData);
        [MonoPInvokeCallback(typeof(LobbyMemberRemovedCallback))]
        public static void LobbyMemberRemovedCallback_Handler(ulong lobbyId,
                                                              ulong memberId,
                                                              void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyMemberRemovedCallback>(__userData);
            try
            {
                __callback(lobbyId, memberId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyMemberUpdatedCallback(ulong lobbyId,
                                                        ulong memberId,
                                                        void* __userData);
        [MonoPInvokeCallback(typeof(LobbyMemberUpdatedCallback))]
        public static void LobbyMemberUpdatedCallback_Handler(ulong lobbyId,
                                                              ulong memberId,
                                                              void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyMemberUpdatedCallback>(__userData);
            try
            {
                __callback(lobbyId, memberId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LobbyUpdatedCallback(ulong lobbyId, void* __userData);
        [MonoPInvokeCallback(typeof(LobbyUpdatedCallback))]
        public static void LobbyUpdatedCallback_Handler(ulong lobbyId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.LobbyUpdatedCallback>(__userData);
            try
            {
                __callback(lobbyId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AcceptActivityInviteCallback(ClientResult* result,
                                                          Discord_String joinSecret,
                                                          void* __userData);
        [MonoPInvokeCallback(typeof(AcceptActivityInviteCallback))]
        public static void AcceptActivityInviteCallback_Handler(ClientResult* result,
                                                                Discord_String joinSecret,
                                                                void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.AcceptActivityInviteCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0),
                           Marshal.PtrToStringUTF8((IntPtr)joinSecret.ptr, (int)joinSecret.size));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(joinSecret.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SendActivityInviteCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(SendActivityInviteCallback))]
        public static void SendActivityInviteCallback_Handler(ClientResult* result,
                                                              void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.SendActivityInviteCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ActivityInviteCallback(ActivityInvite* invite, void* __userData);
        [MonoPInvokeCallback(typeof(ActivityInviteCallback))]
        public static void ActivityInviteCallback_Handler(ActivityInvite* invite,
                                                          void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.ActivityInviteCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ActivityInvite(*invite, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ActivityJoinCallback(Discord_String joinSecret, void* __userData);
        [MonoPInvokeCallback(typeof(ActivityJoinCallback))]
        public static void ActivityJoinCallback_Handler(Discord_String joinSecret,
                                                        void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.ActivityJoinCallback>(__userData);
            try
            {
                __callback(Marshal.PtrToStringUTF8((IntPtr)joinSecret.ptr, (int)joinSecret.size));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
                Discord_Free(joinSecret.ptr);
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateStatusCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(UpdateStatusCallback))]
        public static void UpdateStatusCallback_Handler(ClientResult* result, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UpdateStatusCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateRichPresenceCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(UpdateRichPresenceCallback))]
        public static void UpdateRichPresenceCallback_Handler(ClientResult* result,
                                                              void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UpdateRichPresenceCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UpdateRelationshipCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(UpdateRelationshipCallback))]
        public static void UpdateRelationshipCallback_Handler(ClientResult* result,
                                                              void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UpdateRelationshipCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SendFriendRequestCallback(ClientResult* result, void* __userData);
        [MonoPInvokeCallback(typeof(SendFriendRequestCallback))]
        public static void SendFriendRequestCallback_Handler(ClientResult* result,
                                                             void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.SendFriendRequestCallback>(__userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RelationshipCreatedCallback(ulong userId,
                                                         bool isDiscordRelationshipUpdate,
                                                         void* __userData);
        [MonoPInvokeCallback(typeof(RelationshipCreatedCallback))]
        public static void RelationshipCreatedCallback_Handler(ulong userId,
                                                               bool isDiscordRelationshipUpdate,
                                                               void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.RelationshipCreatedCallback>(__userData);
            try
            {
                __callback(userId, isDiscordRelationshipUpdate);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RelationshipDeletedCallback(ulong userId,
                                                         bool isDiscordRelationshipUpdate,
                                                         void* __userData);
        [MonoPInvokeCallback(typeof(RelationshipDeletedCallback))]
        public static void RelationshipDeletedCallback_Handler(ulong userId,
                                                               bool isDiscordRelationshipUpdate,
                                                               void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.RelationshipDeletedCallback>(__userData);
            try
            {
                __callback(userId, isDiscordRelationshipUpdate);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GetDiscordClientConnectedUserCallback(ClientResult* result,
                                                                   UserHandle* user,
                                                                   void* __userData);
        [MonoPInvokeCallback(typeof(GetDiscordClientConnectedUserCallback))]
        public static void GetDiscordClientConnectedUserCallback_Handler(ClientResult* result,
                                                                         UserHandle* user,
                                                                         void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.GetDiscordClientConnectedUserCallback>(
                  __userData);
            try
            {
                __callback(new Discord.SocialSdk.ClientResult(*result, 0),
                           user == null ? null : new Discord.SocialSdk.UserHandle(*user, 0));
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UserUpdatedCallback(ulong userId, void* __userData);
        [MonoPInvokeCallback(typeof(UserUpdatedCallback))]
        public static void UserUpdatedCallback_Handler(ulong userId, void* __userData)
        {
            var __callback =
              ManagedUserData
                .DelegateFromPointer<SocialSdk.Client.UserUpdatedCallback>(__userData);
            try
            {
                __callback(userId);
            }
            catch (Exception ex)
            {
                __ReportUnhandledException(ex);
            }
            finally
            {
            }
        }
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_Init",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_InitWithBases",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitWithBases(Client* self,
                                                Discord_String apiBase,
                                                Discord_String webBase);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ErrorToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ErrorToString(SocialSdk.Client.Error type,
                                                Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetApplicationId(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetDefaultAudioDeviceId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDefaultAudioDeviceId(Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetDefaultCommunicationScopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDefaultCommunicationScopes(Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetDefaultPresenceScopes",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDefaultPresenceScopes(Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetVersionHash",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetVersionHash(Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetVersionMajor",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVersionMajor();
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetVersionMinor",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVersionMinor();
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetVersionPatch",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVersionPatch();
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_StatusToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatusToString(SocialSdk.Client.Status type,
                                                 Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ThreadToString",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ThreadToString(SocialSdk.Client.Thread type,
                                                 Discord_String* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_EndCall",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void EndCall(Client* self,
                                          ulong channelId,
                                          EndCallCallback callback,
                                          void* callback__userDataFree,
                                          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_EndCalls",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void EndCalls(
          Client* self,
          EndCallsCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetCall",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetCall(Client* self, ulong channelId, Call* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetCalls",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCalls(Client* self, Discord_CallSpan* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetCurrentInputDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCurrentInputDevice(
          Client* self,
          GetCurrentInputDeviceCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetCurrentOutputDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCurrentOutputDevice(
          Client* self,
          GetCurrentOutputDeviceCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetInputDevices",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetInputDevices(
          Client* self,
          GetInputDevicesCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetInputVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetInputVolume(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetOutputDevices",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetOutputDevices(
          Client* self,
          GetOutputDevicesCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetOutputVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetOutputVolume(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetSelfDeafAll",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSelfDeafAll(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetSelfMuteAll",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSelfMuteAll(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetAutomaticGainControl",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAutomaticGainControl(Client* self, bool on);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetDeviceChangeCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDeviceChangeCallback(
          Client* self,
          DeviceChangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetEchoCancellation",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetEchoCancellation(Client* self, bool on);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetInputDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInputDevice(
          Client* self,
          Discord_String deviceId,
          SetInputDeviceCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetInputVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInputVolume(Client* self, float inputVolume);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetNoAudioInputCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNoAudioInputCallback(
          Client* self,
          NoAudioInputCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetNoAudioInputThreshold",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNoAudioInputThreshold(Client* self, float dBFSThreshold);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetNoiseSuppression",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNoiseSuppression(Client* self, bool on);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetOpusHardwareCoding",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOpusHardwareCoding(Client* self, bool encode, bool decode);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetOutputDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOutputDevice(
          Client* self,
          Discord_String deviceId,
          SetOutputDeviceCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetOutputVolume",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOutputVolume(Client* self, float outputVolume);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetSelfDeafAll",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSelfDeafAll(Client* self, bool deaf);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetSelfMuteAll",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSelfMuteAll(Client* self, bool mute);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetSpeakerMode",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetSpeakerMode(Client* self, bool speakerMode);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetThreadPriority",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetThreadPriority(Client* self,
                                                    SocialSdk.Client.Thread thread,
                                                    int priority);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetVoiceParticipantChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetVoiceParticipantChangedCallback(
          Client* self,
          VoiceParticipantChangedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ShowAudioRoutePicker",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ShowAudioRoutePicker(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_StartCall",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartCall(Client* self, ulong channelId, Call* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_StartCallWithAudioCallbacks",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartCallWithAudioCallbacks(
          Client* self,
          ulong lobbyId,
          UserAudioReceivedCallback receivedCb,
          void* receivedCb__userDataFree,
          void* receivedCb__userData,
          UserAudioCapturedCallback capturedCb,
          void* capturedCb__userDataFree,
          void* capturedCb__userData,
          Call* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AbortAuthorize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AbortAuthorize(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AbortGetTokenFromDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AbortGetTokenFromDevice(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_Authorize",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Authorize(
          Client* self,
          AuthorizationArgs* args,
          AuthorizationCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CloseAuthorizeDeviceScreen",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseAuthorizeDeviceScreen(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CreateAuthorizationCodeVerifier",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateAuthorizationCodeVerifier(
          Client* self,
          AuthorizationCodeVerifier* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_FetchCurrentUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void FetchCurrentUser(
          Client* self,
          AuthorizationTokenType tokenType,
          Discord_String token,
          FetchCurrentUserCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetProvisionalToken",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetProvisionalToken(
          Client* self,
          ulong applicationId,
          AuthenticationExternalAuthType externalAuthType,
          Discord_String externalAuthToken,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetToken",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetToken(
          Client* self,
          ulong applicationId,
          Discord_String code,
          Discord_String codeVerifier,
          Discord_String redirectUri,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetTokenFromDevice",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTokenFromDevice(
          Client* self,
          DeviceAuthorizationArgs* args,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetTokenFromDeviceProvisionalMerge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTokenFromDeviceProvisionalMerge(
          Client* self,
          DeviceAuthorizationArgs* args,
          AuthenticationExternalAuthType externalAuthType,
          Discord_String externalAuthToken,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetTokenFromProvisionalMerge",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTokenFromProvisionalMerge(
          Client* self,
          ulong applicationId,
          Discord_String code,
          Discord_String codeVerifier,
          Discord_String redirectUri,
          AuthenticationExternalAuthType externalAuthType,
          Discord_String externalAuthToken,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_IsAuthenticated",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsAuthenticated(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_OpenAuthorizeDeviceScreen",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenAuthorizeDeviceScreen(Client* self,
                                                            ulong clientId,
                                                            Discord_String userCode);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ProvisionalUserMergeCompleted",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ProvisionalUserMergeCompleted(Client* self, bool success);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RefreshToken",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RefreshToken(
          Client* self,
          ulong applicationId,
          Discord_String refreshToken,
          TokenExchangeCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetAuthorizeDeviceScreenClosedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAuthorizeDeviceScreenClosedCallback(
          Client* self,
          AuthorizeDeviceScreenClosedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetGameWindowPid",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGameWindowPid(Client* self, int pid);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetTokenExpirationCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTokenExpirationCallback(
          Client* self,
          TokenExpirationCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UpdateProvisionalAccountDisplayName",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateProvisionalAccountDisplayName(
          Client* self,
          Discord_String name,
          UpdateProvisionalAccountDisplayNameCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UpdateToken",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateToken(
          Client* self,
          AuthorizationTokenType tokenType,
          Discord_String token,
          UpdateTokenCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CanOpenMessageInDiscord",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CanOpenMessageInDiscord(Client* self, ulong messageId);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_DeleteUserMessage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteUserMessage(
          Client* self,
          ulong recipientId,
          ulong messageId,
          DeleteUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_EditUserMessage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void EditUserMessage(
          Client* self,
          ulong recipientId,
          ulong messageId,
          Discord_String content,
          EditUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetChannelHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetChannelHandle(Client* self,
                                                   ulong channelId,
                                                   ChannelHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetMessageHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetMessageHandle(Client* self,
                                                   ulong messageId,
                                                   MessageHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_OpenMessageInDiscord",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenMessageInDiscord(
          Client* self,
          ulong messageId,
          ProvisionalUserMergeRequiredCallback provisionalUserMergeRequiredCallback,
          void* provisionalUserMergeRequiredCallback__userDataFree,
          void* provisionalUserMergeRequiredCallback__userData,
          OpenMessageInDiscordCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendLobbyMessage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendLobbyMessage(
          Client* self,
          ulong lobbyId,
          Discord_String content,
          SendUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendLobbyMessageWithMetadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendLobbyMessageWithMetadata(
          Client* self,
          ulong lobbyId,
          Discord_String content,
          Discord_Properties metadata,
          SendUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendUserMessage",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendUserMessage(
          Client* self,
          ulong recipientId,
          Discord_String content,
          SendUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendUserMessageWithMetadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendUserMessageWithMetadata(
          Client* self,
          ulong recipientId,
          Discord_String content,
          Discord_Properties metadata,
          SendUserMessageCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetMessageCreatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMessageCreatedCallback(
          Client* self,
          MessageCreatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetMessageDeletedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMessageDeletedCallback(
          Client* self,
          MessageDeletedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetMessageUpdatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMessageUpdatedCallback(
          Client* self,
          MessageUpdatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetShowingChat",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetShowingChat(Client* self, bool showingChat);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AddLogCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddLogCallback(
          Client* self,
          LogCallback callback,
          void* callback__userDataFree,
          void* callback__userData,
          LoggingSeverity minSeverity);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AddVoiceLogCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddVoiceLogCallback(
          Client* self,
          LogCallback callback,
          void* callback__userDataFree,
          void* callback__userData,
          LoggingSeverity minSeverity);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_Connect",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Connect(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_Disconnect",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetStatus",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern SocialSdk.Client.Status GetStatus(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetApplicationId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetApplicationId(Client* self, ulong applicationId);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLogDir",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetLogDir(Client* self,
                                            Discord_String path,
                                            LoggingSeverity minSeverity);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetStatusChangedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStatusChangedCallback(
          Client* self,
          OnStatusChanged cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetVoiceLogDir",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetVoiceLogDir(Client* self,
                                                 Discord_String path,
                                                 LoggingSeverity minSeverity);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CreateOrJoinLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateOrJoinLobby(
          Client* self,
          Discord_String secret,
          CreateOrJoinLobbyCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CreateOrJoinLobbyWithMetadata",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateOrJoinLobbyWithMetadata(
          Client* self,
          Discord_String secret,
          Discord_Properties lobbyMetadata,
          Discord_Properties memberMetadata,
          CreateOrJoinLobbyCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetGuildChannels",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetGuildChannels(
          Client* self,
          ulong guildId,
          GetGuildChannelsCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetLobbyHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLobbyHandle(Client* self,
                                                 ulong lobbyId,
                                                 LobbyHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetLobbyIds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetLobbyIds(Client* self, Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetUserGuilds",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetUserGuilds(
          Client* self,
          GetUserGuildsCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_LeaveLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void LeaveLobby(
          Client* self,
          ulong lobbyId,
          LeaveLobbyCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_LinkChannelToLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void LinkChannelToLobby(
          Client* self,
          ulong lobbyId,
          ulong channelId,
          LinkOrUnlinkChannelCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyCreatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyCreatedCallback(
          Client* self,
          LobbyCreatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyDeletedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyDeletedCallback(
          Client* self,
          LobbyDeletedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyMemberAddedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyMemberAddedCallback(
          Client* self,
          LobbyMemberAddedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyMemberRemovedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyMemberRemovedCallback(
          Client* self,
          LobbyMemberRemovedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyMemberUpdatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyMemberUpdatedCallback(
          Client* self,
          LobbyMemberUpdatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetLobbyUpdatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLobbyUpdatedCallback(
          Client* self,
          LobbyUpdatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UnlinkChannelFromLobby",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnlinkChannelFromLobby(
          Client* self,
          ulong lobbyId,
          LinkOrUnlinkChannelCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AcceptActivityInvite",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AcceptActivityInvite(
          Client* self,
          ActivityInvite* invite,
          AcceptActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_ClearRichPresence",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearRichPresence(Client* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RegisterLaunchCommand",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RegisterLaunchCommand(Client* self,
                                                        ulong applicationId,
                                                        Discord_String command);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RegisterLaunchSteamApplication",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RegisterLaunchSteamApplication(Client* self,
                                                                 ulong applicationId,
                                                                 uint steamAppId);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendActivityInvite",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendActivityInvite(
          Client* self,
          ulong userId,
          Discord_String content,
          SendActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendActivityJoinRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendActivityJoinRequest(
          Client* self,
          ulong userId,
          SendActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendActivityJoinRequestReply",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendActivityJoinRequestReply(
          Client* self,
          ActivityInvite* invite,
          SendActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetActivityInviteCreatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetActivityInviteCreatedCallback(
          Client* self,
          ActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetActivityInviteUpdatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetActivityInviteUpdatedCallback(
          Client* self,
          ActivityInviteCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetActivityJoinCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetActivityJoinCallback(
          Client* self,
          ActivityJoinCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetOnlineStatus",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOnlineStatus(
          Client* self,
          StatusType status,
          UpdateStatusCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UpdateRichPresence",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateRichPresence(
          Client* self,
          Activity* activity,
          UpdateRichPresenceCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AcceptDiscordFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AcceptDiscordFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_AcceptGameFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void AcceptGameFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_BlockUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void BlockUser(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CancelDiscordFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CancelDiscordFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_CancelGameFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void CancelGameFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetRelationshipHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetRelationshipHandle(Client* self,
                                                        ulong userId,
                                                        RelationshipHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetRelationships",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetRelationships(Client* self,
                                                   Discord_RelationshipHandleSpan* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RejectDiscordFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RejectDiscordFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RejectGameFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RejectGameFriendRequest(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RemoveDiscordAndGameFriend",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemoveDiscordAndGameFriend(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_RemoveGameFriend",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemoveGameFriend(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SearchFriendsByUsername",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SearchFriendsByUsername(Client* self,
                                                          Discord_String searchStr,
                                                          Discord_UserHandleSpan* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendDiscordFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendDiscordFriendRequest(
          Client* self,
          Discord_String username,
          SendFriendRequestCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendDiscordFriendRequestById",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendDiscordFriendRequestById(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendGameFriendRequest",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendGameFriendRequest(
          Client* self,
          Discord_String username,
          SendFriendRequestCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SendGameFriendRequestById",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendGameFriendRequestById(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetRelationshipCreatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRelationshipCreatedCallback(
          Client* self,
          RelationshipCreatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetRelationshipDeletedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetRelationshipDeletedCallback(
          Client* self,
          RelationshipDeletedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_UnblockUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void UnblockUser(
          Client* self,
          ulong userId,
          UpdateRelationshipCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetCurrentUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetCurrentUser(Client* self, UserHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetDiscordClientConnectedUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDiscordClientConnectedUser(
          Client* self,
          ulong applicationId,
          GetDiscordClientConnectedUserCallback callback,
          void* callback__userDataFree,
          void* callback__userData);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_GetUser",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetUser(Client* self, ulong userId, UserHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_Client_SetUserUpdatedCallback",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetUserUpdatedCallback(
          Client* self,
          UserUpdatedCallback cb,
          void* cb__userDataFree,
          void* cb__userData);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CallInfoHandle
    {
        public IntPtr Handle;
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_Drop",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Drop(CallInfoHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_Clone",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void Clone(CallInfoHandle* self, CallInfoHandle* other);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_ChannelId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong ChannelId(CallInfoHandle* self);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_GetParticipants",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetParticipants(CallInfoHandle* self,
                                                  Discord_UInt64Span* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_GetVoiceStateHandle",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetVoiceStateHandle(CallInfoHandle* self,
                                                      ulong userId,
                                                      VoiceStateHandle* returnValue);
        [DllImport(LibraryName,
                   EntryPoint = "Discord_CallInfoHandle_GuildId",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GuildId(CallInfoHandle* self);
    }
}
