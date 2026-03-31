using System.Runtime.InteropServices;

namespace Meadow.Core
{
    // meadow_idle_monitor_get_value
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            /// <summary>
            /// Blittable layout-compatible struct for the native mallinfo() return value.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct NativeAllocationInfo
            {
                public int Arena;
                public int FreeBlocks;
                public int LargestFreeBlock;
                public int TotalAllocated;
                public int TotalFree;
            }

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial NativeAllocationInfo mallinfo();

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int meadow_idle_monitor_get_value();
        }
    }
}
