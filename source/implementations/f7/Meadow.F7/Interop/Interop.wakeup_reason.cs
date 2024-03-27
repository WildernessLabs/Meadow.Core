using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int pwrmgmt_most_recent_wakeup_reason();

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int meadow_os_native_protocol_version();
        }
    }
}
