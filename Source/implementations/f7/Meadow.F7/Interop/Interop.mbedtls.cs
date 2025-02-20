using System.Runtime.InteropServices;
namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(MBEDTLS_LIBRARY_NAME, SetLastError = true)]
        public static extern int mono_mbedtls_set_server_cert_authmode(int authmode);
    }
}