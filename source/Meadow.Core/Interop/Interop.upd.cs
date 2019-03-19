using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum UpdIoctlFn
            {
                SetRegister = 1,
                GetRegister = 2,
                UpdateRegister = 3,
                RegisterGpioIrq = 5,
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

            public struct UpdGpioInterruptConfiguration
            {
                public int Irq;
                public int Port;
                public int Pin;
                public bool Enable;
                public bool RisingEdge;
                public bool FallingEdge;
            }

            /*
            struct gpio_int_config
            {
              uint32_t irq;
              uint32_t port;
              uint32_t pin;
              bool enable;
              bool risingEdge;
              bool fallingEdge;
            };

            struct upd_config_interrupt
            {
              uint32_t interruptType;

              union cfg
              {
                struct gpio_int_config gpio;
                void *foo;
              } cfg;
            };
            */
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

