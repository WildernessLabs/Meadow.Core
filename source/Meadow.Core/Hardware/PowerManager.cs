using Meadow.Devices;
using System;
using System.IO;
using static Meadow.Core.Interop;

namespace Meadow
{
    public class PowerManager : IPowerManager
    {
        public PowerManager()
        {
        }

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

        public void WatchdogEnable(int timeoutMs)
        {
            Console.WriteLine($"! Watchdog Enable {timeoutMs}ms");

            // we'll use the IWDG registers.  
            // WWDG is too fast, with a max timeout in the 40ms range, which for a managed app is a recipe for lots of restarts
            // the IWDG uses the LSI clock, which has a freq of 32000
            // if we set the prescaler divider at /32, we end up with a simple 1ms per tick

            // from the data sheet:
            //1. Enable the IWDG by writing 0x0000 CCCC in the Key register (IWDG_KR).
            //2. Enable register access by writing 0x0000 5555 in the Key register (IWDG_KR).
            //3. Write the prescaler by programming the Prescaler register (IWDG_PR) from 0 to 7.
            //4. Write the Reload register (IWDG_RLR).
            //5. Wait for the registers to be updated (IWDG_SR = 0x0000 0000).
            //6. Refresh the counter value with IWDG_RLR (IWDG_KR = 0x0000 AAAA)

            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000cccc);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x00005555);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_PR_OFFSET, STM32.IWDG_PR_DIV_64);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_RLR_OFFSET, (uint)timeoutMs);
        }

        public void WatchdogReset()
        {
            Console.WriteLine("! Watchdog Reset !");
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000aaaa);
        }
    }
}
