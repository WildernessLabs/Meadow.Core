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
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerSleep);
        }
    }
}
