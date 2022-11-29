using Meadow.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
            InitializeStorage();
        }
    }
}
