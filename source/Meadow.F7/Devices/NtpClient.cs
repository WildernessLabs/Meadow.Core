using System;

namespace Meadow
{
    /// <summary>
    /// Client for Network Time Protocol
    /// </summary>
    public class NtpClient : INtpClient
    {
        /// <summary>
        /// Returns <b>true</b> if the NTP Client is enabled
        /// </summary>
        public bool Enabled => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup);

        internal NtpClient()
        {
        }

        /// <summary>
        /// Time period that the NTP client attempts to query the NTP time server(s)
        /// </summary>
        public TimeSpan PollPeriod
        {
            get => TimeSpan.Zero; // currently only happens at startup
            set => throw new PlatformNotSupportedException("Changing NTP Poll Frequency not currently supported");
        }

        /// <summary>
        /// Event raised when the device clock is adjusted by NTP
        /// </summary>
        public event TimeChangedEventHandler TimeChanged = delegate { };

        internal void RaiseTimeChanged()
        {
            TimeChanged?.Invoke(DateTime.UtcNow);
        }
    }
}
