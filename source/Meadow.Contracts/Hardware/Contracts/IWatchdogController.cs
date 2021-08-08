using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for hardware that has a watchdog. Watchdogs are used to reset
    /// a device in the event that the application code on it has stopped responding.
    /// </summary>
    public interface IWatchdogController
    {
        /// <summary>
        /// Enables the watchdog. If `WatchdogReset` isn't called within the
        /// specified `timeout` the device will reset.
        /// </summary>
        /// <param name="timeout"></param>
        void WatchdogEnable(TimeSpan timeout);

        /// <summary>
        /// "Pets" the watchdog. By calling this within the `timeout`
        /// specified in the `WatchdogEnable` method, the device will not reset.
        /// </summary>
        void WatchdogReset();
    }
}
