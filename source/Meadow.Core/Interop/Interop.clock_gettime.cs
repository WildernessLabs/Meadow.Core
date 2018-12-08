using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport("nuttx", SetLastError = true)]
            public static extern int clock_gettime(clockid_t clk_id, ref timespec tp);
        }
    }
}
