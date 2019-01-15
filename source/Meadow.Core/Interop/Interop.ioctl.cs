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

            /// <summary>
            /// Reads an input
            /// </summary>
            /// <returns>The ioctl.</returns>
            /// <param name="fd">Fd.</param>
            /// <param name="request">Request.</param>
            /// <param name="pinDesignator">Pin designator.</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref int pinDesignator);

            /// <summary>
            /// Writes a Discrete Output
            /// </summary>
            /// <returns>The ioctl.</returns>
            /// <param name="fd">Fd.</param>
            /// <param name="request">Request.</param>
            /// <param name="pinState">Pin state.</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOPinState pinState);

            /// <summary>
            /// Configures a pin
            /// </summary>
            /// <returns>The ioctl.</returns>
            /// <param name="fd">Fd.</param>
            /// <param name="request">Request.</param>
            /// <param name="configFlags">Config flags.</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOConfigFlags configFlags);

        }
    }
}
