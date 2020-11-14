using Meadow.Devices;
using System;
using static Meadow.Core.Interop;

namespace Meadow
{
    public class PowerManager : IPowerManager
    {
        public unsafe void Reset()
        {
            Console.WriteLine("! Software Reset Requested !");
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

        public void Sleep()
        {
            Console.WriteLine("! Software Sleep Requested !");
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerSleep1);
        }

        public void WatchdogEnable(ulong timoutMs)
        {
            Console.WriteLine("! Watchdog Enable !");
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerWDSet, ref timoutMs);
        }

        public void WatchdogReset()
        {
            Console.WriteLine("! Watchdog Reset !");
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerWDPet);
        }
    }
}
