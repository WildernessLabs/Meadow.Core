using System;
using Meadow.Devices;

namespace Meadow
{
    public static partial class MeadowOS
    {
        public static IMeadowDevice CurrentDevice { get; set; } = null!;


        public static void Sleep(DateTime until)
        {
            throw new NotImplementedException();
        }

        public static void Sleep(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public static void Sleep(WakeUpOptions wakeUp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the Meadow hardware immediately
        /// </summary>
        [Obsolete("Use CurrentDevice.Reset", false)]
        public static void Reset()
        {
            CurrentDevice.Reset();
        }

        /// <summary>
        /// Enables a watchdog timer with the specified timeout in milliseconds.
        /// If Watchdog.Reset is not called before the timeout period, the Meadow
        /// will reset.
        /// </summary>
        /// <param name="timeoutMs">Watchdog timeout period, in milliseconds.
        /// Maximum allowed timeout of 32,768ms</param>
        [Obsolete("Use CurrentDevice.Reset", false)]
        public static void WatchdogEnable(int timeoutMs)
        {
            CurrentDevice.WatchdogEnable(TimeSpan.FromMilliseconds(timeoutMs));
        }

        public static void WatchdogReset()
        {
            CurrentDevice.WatchdogReset();
        }
    }
}
