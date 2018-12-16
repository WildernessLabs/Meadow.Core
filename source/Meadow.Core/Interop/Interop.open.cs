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


            [DllImport(LIBRARY_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int open_void(string pathname, int flags);

            [DllImport(LIBRARY_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr open(string pathname, DriverFlags flags, int param);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int close(IntPtr ptr);


            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, int request, IntPtr stuff);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, int request, out GPIOPinType pinType);
        }
    }
}
