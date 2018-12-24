using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum DriverFlags
            {
                ReadOnly = 0,   // O_RDONLY
                WriteOnly = 1,  // O_WRONLY
                ReadWrite = 2   // O_RDWR
            }

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern IntPtr open(string pathname, DriverFlags flags);
        }
    }
}
