using Meadow.Units;
using System;

namespace Meadow.Simulation
{
    public class SimulatedPlatformOS : IPlatformOS
    {
        public event PowerTransitionHandler BeforeReset;
        public event PowerTransitionHandler BeforeSleep;
        public event PowerTransitionHandler AfterWake;

        public string OSVersion => "0.1";

        public string OSBuildDate => throw new NotImplementedException();

        public string MonoVersion => throw new NotImplementedException();

        public bool RebootOnUnhandledException => throw new NotImplementedException();

        public uint InitializationTimeout => throw new NotImplementedException();

        public bool SdCardPresent => throw new NotImplementedException();

        public INtpClient NtpClient => throw new NotImplementedException();

        public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
        {
            throw new NotImplementedException();
        }

        public Temperature GetCpuTemperature()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
        {
            throw new NotImplementedException();
        }

        public void Sleep(TimeSpan duration)
        {
            throw new NotImplementedException();
        }
    }
}
