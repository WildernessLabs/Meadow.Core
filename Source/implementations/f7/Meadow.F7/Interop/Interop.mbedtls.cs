using System.Runtime.InteropServices;
namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [LibraryImport(MBEDTLS_LIBRARY_NAME, SetLastError = true)]
        public static partial int mono_mbedtls_set_server_cert_authmode(int authmode);
    }
}