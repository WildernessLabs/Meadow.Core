using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum GpioIoctlFn
            {
                SetConfig = 1,
                Write = 3,
                Read = 4,
            }

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref bool value);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOPinState pinState);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOConfigFlags configFlags);

        }
    }
}
