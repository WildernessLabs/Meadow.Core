using System;
using System.Threading.Tasks;

namespace Meadow
{
    public class LinuxNtpClient : INtpClient
    {
        public bool Enabled => false;

        public TimeSpan PollPeriod { get; set; }

        public event TimeChangedEventHandler TimeChanged;

        public Task<bool> Synchronize(string? ntpServer = null)
        {
            throw new NotImplementedException();
        }
    }
}
