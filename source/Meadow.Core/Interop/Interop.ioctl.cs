using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Core
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
            /// Reads or Writes a Universal Platform Driver register
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant</param>
            /// <param name="registerValue">A RegisterValue struct to use as either the value soure (for writes) or sink (for reads)</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdRegisterValue registerValue);

            /// <summary>
            /// Reads or Writes a Universal Platform Driver register
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant</param>
            /// <param name="registerUpdate">A RegisterUpdate struct describing which bits to set and clear in a single register</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdRegisterUpdate registerUpdate);

            /// <summary>
            /// Configures the Universal Platofrm Driver to catch GPIO interrupts
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant - should be 5 in this case</param>
            /// <param name="interruptConfig">A UpdGpioInterruptConfiguration struct describing the specific interrupt configuration desired</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdGpioInterruptConfiguration interruptConfig);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdI2CCommand i2cCommand);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPIDataCommand spiCommand);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPISpeedCommand spiCommand);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPIModeCommand spiCommand);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, IntPtr pData);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, StringBuilder sb);

            /// <summary>
            /// Configures the Universal Platofrm Driver to set PWM APB clock.
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant - should be 5 in this case</param>
            /// <param name="pwmCmd">A UpdPwmCmd struct describing the specific PWM command</param>
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdPwmCmd pwmCmd);

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
