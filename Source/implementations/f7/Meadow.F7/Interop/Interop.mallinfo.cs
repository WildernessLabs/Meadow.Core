using System.Runtime.InteropServices;

namespace Meadow.Core
{
    // meadow_idle_monitor_get_value
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial AllocationInfo mallinfo();


            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int meadow_idle_monitor_get_value();
        }
    }
}
