using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {
        public static partial class STM32
        {
            public const int GPIOA_BASE = 0x40020000;
            public const int GPIOB_BASE = 0x40020400;
            public const int GPIOC_BASE = 0x40020800;
            public const int GPIOD_BASE = 0x40020c00;
            public const int GPIOE_BASE = 0x40021000;
            public const int GPIOF_BASE = 0x40021400;
            public const int GPIOG_BASE = 0x40021800;
            public const int GPIOH_BASE = 0x40021c00;
            public const int GPIOI_BASE = 0x40022000;
            public const int GPIOJ_BASE = 0x40022400;
            public const int GPIOK_BASE = 0x40022800;

            public const int STM32_GPIO_MODER_OFFSET = 0x00;
            public const int STM32_GPIO_OTYPER_OFFSET = 0x04;
            public const int STM32_GPIO_OSPEED_OFFSET = 0x08;
            public const int STM32_GPIO_PUPDR_OFFSET = 0x0c;
            public const int STM32_GPIO_IDR_OFFSET = 0x10;
            public const int STM32_GPIO_BSRR_OFFSET = 0x18;

            public enum UpdIoctlFn
            {
                SetRegister = 1,
                GetRegister = 2,
                UpdateRegister = 3,
                RegisterIrq = 5,
            }

            public struct UpdRegisterValue
            {
                public uint Address;
                public uint Value;
            }

            public struct UpdRegisterUpdate
            {
                public uint Address;
                public uint ClearBits;
                public uint SetBits;
            }

            public static bool TryGetRegister(IntPtr driverHandle, int address, out uint value)
            {
                return TryGetRegister(driverHandle, (uint)address, out value);
            }

            public static bool TryGetRegister(IntPtr driverHandle, uint address, out uint value)
            {
                var register = new UpdRegisterValue
                {
                    Address = address
                };
                //                Console.WriteLine($"Reading register: {register.Address:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.GetRegister, ref register);
                if (result != 0)
                {
                    Console.WriteLine($"Read failed: {result}");
                    value = (uint)result;
                    return false;
                }
//                Console.WriteLine($"Value: {register.Value:X}");
                value = register.Value;
                return true;
            }

            public static bool SetRegister(IntPtr driverHandle, int address, uint value)
            {
                return SetRegister(driverHandle, (uint)address, value);
            }

            public static bool SetRegister(IntPtr driverHandle, uint address, uint value)
            {
                var register = new UpdRegisterValue
                {
                    Address = address,
                    Value = value
                };
                //                Console.WriteLine($"Writing {register.Value:X} to register: {register.Address:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.SetRegister, ref register);
                if (result != 0)
                {
                    Console.WriteLine($"Write failed: {result}");
                    return false;
                }
                return true;

            }

            public static bool UpdateRegister(IntPtr driverHandle, int address, uint clearBits, uint setBits)
            {
                return UpdateRegister(driverHandle, (uint)address, clearBits, setBits);
            }

            public static bool UpdateRegister(IntPtr driverHandle, uint address, uint clearBits, uint setBits)
            {
                var update = new UpdRegisterUpdate()
                {
                    Address = address,
                    ClearBits = clearBits,
                    SetBits = setBits
                };
//                Console.WriteLine($"Updating register: {update.Address:X} clearing {update.ClearBits:X} setting {update.SetBits:X}");
                var result = Interop.Nuttx.ioctl(driverHandle, UpdIoctlFn.UpdateRegister, ref update);
                if (result != 0)
                {
                    Console.WriteLine($"Update failed: {result}");
                    return false;
                }
                return true;

            }
        }
    }
}

