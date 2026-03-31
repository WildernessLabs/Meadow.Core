using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int pwrmgmt_most_recent_wakeup_reason();

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int meadow_os_native_protocol_version();
        }
    }
}
