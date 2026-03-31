using System;
using System.Runtime.InteropServices;

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
            /// <param name="fd">Driver handle.</param>
            /// <param name="request">Request.</param>
            /// <param name="pinDesignator">Pin designator.</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, GpioIoctlFn request, ref int pinDesignator);

            /// <summary>
            /// Writes a Discrete Output
            /// </summary>
            /// <returns>The ioctl.</returns>
            /// <param name="fd">Driver handle.</param>
            /// <param name="request">Request.</param>
            /// <param name="pinState">Pin state.</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOPinState pinState);

            /// <summary>
            /// Reads or Writes a Universal Platform Driver register
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant</param>
            /// <param name="registerValue">A RegisterValue struct to use as either the value source (for writes) or sink (for reads)</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdRegisterValue registerValue);

            /// <summary>
            /// Reads or Writes a Universal Platform Driver register
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant</param>
            /// <param name="registerUpdate">A RegisterUpdate struct describing which bits to set and clear in a single register</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdRegisterUpdate registerUpdate);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSleepCommand command);

            /// <summary>
            /// Configures the Universal Platform Driver to catch GPIO interrupts
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant - should be 5 in this case</param>
            /// <param name="interruptConfig">A UpdGpioInterruptConfiguration struct describing the specific interrupt configuration desired</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdGpioInterruptConfiguration interruptConfig);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdI2cCommand i2cCommand);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPIDataCommand spiCommand);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPISpeedCommand spiCommand);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPIModeCommand spiCommand);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdSPIBitsCommand spiCommand);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, IntPtr pData);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref int dwData);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdEnumDirCmd command);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, int request, IntPtr pData);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int tcgetattr(IntPtr fd, ref Termios termiosp);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int tcsetattr(IntPtr fd, int options, ref Termios termiosp);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int cfgetspeed(ref Termios termiosp);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int cfsetspeed(ref Termios termiosp, int speed);

            /// <summary>
            /// Configures the Universal Platform Driver to set PWM APB clock.
            /// </summary>
            /// <returns>0 on success, otherwise an error code</returns>
            /// <param name="fd">Driver handle</param>
            /// <param name="request">UPD Ioctl function constant - should be 5 in this case</param>
            /// <param name="pwmCmd">A UpdPwmCmd struct describing the specific PWM command</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdPwmCmd pwmCmd);

            /// <summary>
            /// Configures a pin
            /// </summary>
            /// <returns>The ioctl.</returns>
            /// <param name="fd">Driver handle.</param>
            /// <param name="request">Request.</param>
            /// <param name="configFlags">Config flags.</param>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, GpioIoctlFn request, ref GPIOConfigFlags configFlags);

            /// <summary>
            /// Request the ESP perform a function / execute a command.
            /// </summary>
            /// <param name="fd">File descriptor for the UPD driver.</param>
            /// <param name="request">Function number (should be IoctlFn.Esp32Command).</param>
            /// <param name="espCommand">Data structure holding the information about the command / request.</param>
            /// <returns>0 on success, error code if a problem was encountered.</returns>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdEsp32Command espCommand);

            /// <summary>
            /// Get the extended event data from the STM32.
            /// </summary>
            /// <param name="fd">File descriptor for the UPD driver.</param>
            /// <param name="request">Function number (should be IoctlFn.Esp32GetEventData).</param>
            /// <param name="eventData">Data structure holding the event data.</param>
            /// <returns>0 on success, error code if a problem was encountered.</returns>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdEsp32EventDataPayload eventData);

            /// <summary>
            /// Process a request to read or write a configuration value from the underlying OS.
            /// </summary>
            /// <param name="fd">File descriptor for the UPD driver.</param>
            /// <param name="request">Function number (should be IoctlFn.UpdConfigurationValue).</param>
            /// <param name="eventData">Data structure holding the event data.</param>
            /// <returns>0 on success, error code if a problem was encountered.</returns>
            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref UpdConfigurationValue eventData);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref Nuttx.UpdDeviceInfo deviceInfo);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int ioctl(IntPtr fd, UpdIoctlFn request, ref ulong param);
        }
    }
}
