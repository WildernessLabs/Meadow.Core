using Meadow;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    // meadow_idle_monitor_get_value
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern AllocationInfo mallinfo();


            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int meadow_idle_monitor_get_value();
        }
    }
}
