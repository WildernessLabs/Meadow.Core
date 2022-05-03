using System;
using System.Diagnostics;

namespace Meadow
{
    public class LinuxPlatformOS : IPlatformOS
    {
        public virtual string OSVersion { get; private set; }
        public virtual string OSBuildDate { get; private set; }

        public virtual string MonoVersion => ".NET 5.0"; // TODO"

        public void Initialize()
        {
            try
            {
                var psi = new ProcessStartInfo("/bin/bash", "-c \"lsb_release -d\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                OSVersion = proc.StandardOutput.ReadToEnd().Trim();
            }
            catch
            {
                // TODO:
            }

            try
            {
                var psi = new ProcessStartInfo("/bin/bash", "-c \"uname -v\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                OSBuildDate = proc.StandardOutput.ReadToEnd().Trim();
            }
            catch
            {
                // TODO:
            }
        }

        public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
        {
            throw new NotImplementedException();
        }
    }
}
