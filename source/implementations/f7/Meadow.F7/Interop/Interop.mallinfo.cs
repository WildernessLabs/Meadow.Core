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

namespace Meadow
{

    /// <summary>
    /// A collection of device memory-allocation statistics
    /// </summary>
    public struct AllocationInfo
    {
        /// <summary>
        /// This is the total size of memory allocated for use by malloc in bytes. 
        /// </summary>
        public int Arena { get; set; }
        /// <summary>
        /// This is the number of free (not in use) chunks 
        /// </summary>
        public int FreeBlocks { get; set; }
        /// <summary>
        /// Size of the largest free (not in use) chunk 
        /// </summary>
        public int LargestFreeBlock { get; set; }
        /// <summary>
        /// This is the total size of memory occupied by chunks handed out by malloc. 
        /// </summary>
        public int TotalAllocated { get; set; }
        /// <summary>
        /// This is the total size of memory occupied by free (not in use) chunks.
        /// </summary>
        public int TotalFree { get; set; }
        /*
        struct mallinfo
        {
           int arena;    // This is the total size of memory allocated for use by malloc in bytes. 
           int ordblks;  // This is the number of free (not in use) chunks 
           int mxordblk; // Size of the largest free (not in use) chunk 
           int uordblks; // This is the total size of memory occupied by chunks handed out by malloc. 
           int fordblks; // This is the total size of memory occupied by free (not in use) chunks.
        };
        */
    }
}