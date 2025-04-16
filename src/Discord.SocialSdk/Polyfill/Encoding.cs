#if NETSTANDARD
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.SocialSdk.Polyfill;

public static class EncodingUTF8P
{
    public static unsafe int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        // It's ok for us to operate on null / empty spans.
        fixed (char* charsPtr = &MemoryMarshal.GetReference(chars))
        fixed (byte* bytesPtr = &MemoryMarshal.GetReference(bytes))
        {
            return Encoding.UTF8.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
        }
    }
}
#endif