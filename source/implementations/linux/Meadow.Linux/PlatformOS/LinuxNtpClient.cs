using System;

namespace Meadow
{
    public class LinuxNtpClient : INtpClient
    {
        public bool Enabled => false;

        public TimeSpan PollPeriod { get; set; }

        public event TimeChangedEventHandler TimeChanged;
    }
}
