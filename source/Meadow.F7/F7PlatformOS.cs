using Meadow.Units;
using System;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        public INtpClient NtpClient { get; }

        public F7PlatformOS()
        {
            NtpClient = new NtpClient();
        }

        public Temperature GetCpuTemperature()
        {
            throw new NotSupportedException();
        }

        public void Initialize()
        {

        }
    }
}
