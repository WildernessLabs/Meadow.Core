using System;
using Meadow.Devices;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow
{
    public static class MeadowOS//<D> where D : IIODevice
    {
        internal static void Init(IIODevice device)
        {
            CurrentDevice = device;
        }

        public static IIODevice CurrentDevice { get; private set; }


        public static void Sleep(DateTime until) {
            throw new NotImplementedException();
        }

        public static void Sleep(TimeSpan duration) {
            throw new NotImplementedException();
        }

        public static void Sleep(WakeUpOptions wakeUp) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the Meadow hardware immediately
        /// </summary>
        public static void Reset()
        {
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

        /// <summary>
        /// Enables a watchdog timer with the specified timeout in milliseconds.
        /// If Watchdog.Reset is not called before the timeout period, the Meadow
        /// will reset.
        /// </summary>
        /// <param name="timeoutMs">Watchdog timeout period, in milliseconds.
        /// Maximum allowed timeout of 32,768ms</param>
        public static void WatchdogEnable(int timeoutMs)
        {
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

            // The RLR can have a maximum of 4096, so we have to play with the prescaler to get close to the desired value
            uint prescale, rlr;

            if (timeoutMs < 4096)
            {
                prescale = STM32.IWDG_PR_DIV_32;
                rlr = (uint)timeoutMs;
            }
            else if (timeoutMs < 4096 * 2)
            {
                prescale = STM32.IWDG_PR_DIV_64;
                rlr = (uint)timeoutMs / 2;
            }
            else if (timeoutMs < 4096 * 4)
            {
                prescale = STM32.IWDG_PR_DIV_128;
                rlr = (uint)timeoutMs / 4;
            }
            else if (timeoutMs < 4096 * 8)
            {
                prescale = STM32.IWDG_PR_DIV_256;
                rlr = (uint)timeoutMs / 8;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Timeout must be less than {4096 * 8}ms");
            }

            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000cccc);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x00005555);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_PR_OFFSET, prescale);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_RLR_OFFSET, rlr);
        }

        public static void WatchdogReset()
        {
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000aaaa);
        }
    }
}
