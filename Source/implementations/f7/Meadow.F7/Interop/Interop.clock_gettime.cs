using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int clock_gettime(clockid_t clk_id, ref timespec tp);
        }
    }
}
