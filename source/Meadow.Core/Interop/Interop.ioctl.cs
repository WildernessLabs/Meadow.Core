using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum UpdIoctlFn
            {
                SetRegister = 1,
                GetRegister = 2,
                RegisterIrq = 3,
            }

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
            /// Reads or Writes a Universal Platform Driver register
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant</param>
            /// <param name="registerValue">A RegisterValue struct to use as either the value soure (for writes) or sink (for reads)</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdRegisterValue registerValue);

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
