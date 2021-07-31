using System;
namespace Meadow.Hardware
{
    public interface IWatchdogController
    {
        void WatchdogEnable(TimeSpan timeout);
        void WatchdogReset();
    }
}
