using Meadow.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        /// <summary>
        /// NTP client.
        /// </summary>
        public INtpClient NtpClient { get; }

        /// <summary>
        /// Default construcotr for the F7PlatformOS objects.
        /// </summary>
        public F7PlatformOS()
        {
            NtpClient = new NtpClient();
        }

        /// <summary>
        /// Get the current CPU temperature (Not supported on F7).
        /// </summary>
        /// <exception cref="NotSupportedException">Method is not supported on the F7 platform.</exception>
        public Temperature GetCpuTemperature()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initialize the F7PlatformOS instance.
        /// </summary>
        public void Initialize()
        {
            InitializeStorage();
        }
    }
}
