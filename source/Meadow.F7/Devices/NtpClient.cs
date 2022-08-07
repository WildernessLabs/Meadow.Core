using System;

namespace Meadow.Devices
{
    public class NtpClient : INtpClient
    {
        public bool Enabled => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup);

        public TimeSpan PollFrequency
        {
            get => TimeSpan.Zero; // currently only happens at startup
            set => throw new PlatformNotSupportedException("Changing NTP Poll Frequency not currently supported");
        }

        public event TimeChangedEventHandler TimeChanged = delegate { };

        internal void RaiseTimeChanged()
        {
            TimeChanged?.Invoke(DateTime.UtcNow);
        }
    }
}
