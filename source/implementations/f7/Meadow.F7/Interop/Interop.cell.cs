using System.Runtime.InteropServices;

namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern bool meadow_cell_is_connected();
    }
}
