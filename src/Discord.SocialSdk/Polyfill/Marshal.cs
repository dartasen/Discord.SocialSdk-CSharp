#if NETSTANDARD2_0
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.SocialSdk.Polyfill;

/// <summary>
/// This class contains methods that are mainly used to marshal between unmanaged
/// and managed types.
/// </summary>
public static class MarshalP
{
    public static unsafe string PtrToStringUTF8(IntPtr ptr, int byteLen)
    {
        if (IsNullOrWin32Atom(ptr))
        {
            throw new ArgumentNullException(nameof(ptr));
        }

        if (byteLen < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLen), "byteLen must be non-negative");
        }

        return CreateStringFromEncoding((byte*)ptr, byteLen, Encoding.UTF8);
    }

    /// <summary>
    /// Provides a polyfill for Marshal.StringToCoTaskMemUTF8, which is not available in .NET Standard 2.0.
    /// </summary>
    public static unsafe IntPtr StringToCoTaskMemUTF8(string? value)
    {
        if (value is null)
        {
            return IntPtr.Zero;
        }

        byte[] utf8Bytes = Encoding.UTF8.GetBytes(value);
        IntPtr ptr = Marshal.AllocCoTaskMem(utf8Bytes.Length + 1);
        Marshal.Copy(utf8Bytes, 0, ptr, utf8Bytes.Length);
        Marshal.WriteByte(ptr, utf8Bytes.Length, 0); // Null-terminate the string

        return ptr;
    }

    private static bool IsNullOrWin32Atom(IntPtr ptr)
    {
        // Win32 has the concept of Atoms, where a pointer can either be a pointer
        // or an int.  If it's less than 64K, this is guaranteed to NOT be a
        // pointer since the bottom 64K bytes are reserved in a process' page table.
        // We should be careful about deallocating this stuff.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            const long HIWORDMASK = unchecked((long)0xffffffffffff0000L);
            long lPtr = (long)ptr;

            return 0 == (lPtr & HIWORDMASK);
        }

        return ptr == IntPtr.Zero;
    }

    // Helper for encodings so they can talk to our buffer directly
    // stringLength must be the exact size we'll expect
    private static unsafe string CreateStringFromEncoding(byte* bytes, int byteLength, Encoding encoding)
    {
        Debug.Assert(bytes != null);
        Debug.Assert(byteLength >= 0);

        // Get our string length
        int stringLength = encoding.GetCharCount(bytes, byteLength);
        Debug.Assert(stringLength >= 0);

        // They gave us an empty string if they needed one
        // 0 bytelength might be possible if there's something in an encoder
        if (stringLength == 0)
        {
            return string.Empty;
        }

        string s = new(' ', stringLength); // Initialize with null characters
        fixed (char* pTempChars = s)
        {
            int doubleCheck = encoding.GetChars(bytes, byteLength, pTempChars, stringLength);
            Debug.Assert(stringLength == doubleCheck, "Expected encoding.GetChars to return same length as encoding.GetCharCount");
        }

        return s;
    }
}
#endif