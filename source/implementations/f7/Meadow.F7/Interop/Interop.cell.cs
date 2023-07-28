using System.Runtime.InteropServices;
using System;

namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern bool meadow_cell_is_connected();

        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int meadow_get_cell_pppd_output(IntPtr buf);  
    }
}
