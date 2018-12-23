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

            public enum GpioIoctlFn
            {
                WriteSingle = 1,
                WriteMultiple = 2,
                ReadSingle = 3,
                ReadMultiple = 4,
                GetType = 5,
                SetType = 6
            }


            [DllImport(LIBRARY_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr open(string pathname, DriverFlags flags);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int close(IntPtr ptr);


            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref bool value);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOPinState pinState);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, out GPIOPinType pinType);
        }
    }
}
