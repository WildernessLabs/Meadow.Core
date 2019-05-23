using System;
using Meadow.Core;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    /// <summary>
    /// A wrapped around the Meadow Generic Platform Driver
    /// </summary>
    /// <remarks>
    /// There is one and only one instance of this driver, and it lives effectivly whenever th app is up, so it's
    /// used as a static singleton
    /// </remarks>
    internal static class GPD
    {
        private const string GPDDriverName = "/dev/upd";

        private static IntPtr DriverHandle { get; }

        static GPD()
        {
            DriverHandle = Interop.Nuttx.open(GPDDriverName, Interop.Nuttx.DriverFlags.ReadOnly);
            if (DriverHandle == IntPtr.Zero || DriverHandle.ToInt32() == -1)
            {
                Console.Write("Failed to open UPD driver");
            }
        }

        public static void DumpClockRegisters()
        {
            var cr = GetRegister(STM32.RCC_BASE + STM32.RCC_CR_OFFSET);
            var pllcfg = GetRegister(STM32.RCC_BASE + STM32.RCC_PLLCFGR_OFFSET);
            var ahb1rst = GetRegister(STM32.RCC_BASE + STM32.RCC_AHB1RSTR_OFFSET);
            var ahb1en = GetRegister(STM32.RCC_BASE + STM32.RCC_AHB1ENR_OFFSET);
            var apb1rst = GetRegister(STM32.RCC_BASE + STM32.RCC_APB1RSTR_OFFSET);
            var apb1en = GetRegister(STM32.RCC_BASE + STM32.RCC_APB1ENR_OFFSET);
            var apb2rst = GetRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET);
            var apb2en = GetRegister(STM32.RCC_BASE + STM32.RCC_APB2ENR_OFFSET);
            var dckcfg1 = GetRegister(STM32.RCC_BASE + STM32.RCC_DCKCFGR1_OFFSET);
            var dckcfg2 = GetRegister(STM32.RCC_BASE + STM32.RCC_DCKCFGR2_OFFSET);

            Console.WriteLine("Clock Registers");
            Console.WriteLine($"\tRCC_CR: {cr:X}");
            Console.WriteLine($"\tRCC_AHB1RSTR: {ahb1rst:X}");
            Console.WriteLine($"\tRCC_AHB1ENR: {ahb1en:X}");
            Console.WriteLine($"\tRCC_APB1RSTR: {apb1rst:X}");
            Console.WriteLine($"\tRCC_APB1ENR: {apb1en:X}");
            Console.WriteLine($"\tRCC_APB2RSTR: {apb2rst:X}");
            Console.WriteLine($"\tRCC_APB2ENR: {apb2en:X}");
            Console.WriteLine($"\tRCC_PLLCFGR: {pllcfg:X}");
            Console.WriteLine($"\tRCC_DCKCFGR1: {dckcfg1:X}");
            Console.WriteLine($"\tRCC_DCKCFGR2: {dckcfg2:X}");
        }

        public static void DumpI2CRegisters()
        {
            var cr1 = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET);
            var cr2 = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET);
            var timing = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMINGR_OFFSET);
            var isr = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_ISR_OFFSET);

            Console.WriteLine("I2C Registers");
            Console.WriteLine($"I2C_CR1:{cr1:X}");
            Console.WriteLine($"I2C_CR2:{cr2:X}");
            Console.WriteLine($"I2C_ISR:{isr:X}");
            Console.WriteLine($"I2C_TIMINGR:{timing:X}");
        }

        public static void SetRegister(uint address, uint value)
        {
            Nuttx.SetRegister(DriverHandle, address, value);
        }

        public static uint GetRegister(uint address)
        {
            // we ignore the result here as I've never seen it fail, and if it does, there's zero that the app can do about it
            Nuttx.TryGetRegister(DriverHandle, address, out uint value);
            return value;
        }

        public static void UpdateRegister(uint address, uint clearBits, uint setBits)
        {
            Nuttx.UpdateRegister(DriverHandle, address, clearBits, setBits);
        }

        public static int Ioctl(Nuttx.GpioIoctlFn request, ref int pinDesignator)
        {
            return Nuttx.ioctl(DriverHandle, request, ref pinDesignator);
        }

        public static int Ioctl(Nuttx.GpioIoctlFn request, ref Nuttx.GPIOPinState pinState)
        {
            return Nuttx.ioctl(DriverHandle, request, ref pinState);
        }

        public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdRegisterValue registerValue)
        {
            return Nuttx.ioctl(DriverHandle, request, ref registerValue);
        }

        public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdRegisterUpdate registerUpdate)
        {
            return Nuttx.ioctl(DriverHandle, request, ref registerUpdate);
        }

        public static int Ioctl(Nuttx.UpdIoctlFn request, ref Nuttx.UpdGpioInterruptConfiguration interruptConfig)
        {
            return Nuttx.ioctl(DriverHandle, request, ref interruptConfig);
        }

        public static int Ioctl(Nuttx.GpioIoctlFn request, ref Nuttx.GPIOConfigFlags configFlags)
        {
            return Nuttx.ioctl(DriverHandle, request, ref configFlags);
        }
    }

    /* ===== MEADOW GPIO PIN MAP =====
        BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   IMPLEMENTED?
        J301-1      RESET                       
        J301-2      3.3                       
        J301-3      VREF                       
        J301-4      GND                       
        J301-5      DAC_OUT1        PA4         A0
        J301-6      DAC_OUT2        PA5         A1
        J301-7      ADC1_IN3        PA3         A2
        J301-8      ADC1_IN7        PA7         A3
        J301-9      ADC1_IN10       PC0         A4
        J301-10     ADC1_IN11       PC1         A5
        J301-11     SPI3_CLK        PC10        SCK
        J301-12     SPI3_MOSI       PB5         MOSI    AF6
        J301-13     SPI3_MISO       PC11        MISO    AF6
        J301-14     UART4_RX        PI9         D00     AF8
        J301-15     UART4_TX        PH13        D01     AF8
        J301-16     PC6             PC6         D02                 *
        J301-17     CAN1_RX         PB8         D03     AF9
        J301-18     CAN1_TX         PB9         D04     AF9

        J302-4      PE3             PE3         D15
        J302-5      PG3             PG3         D14
        J302-6      USART1_RX       PB15        D13     AF4
        J302-7      USART1_TX       PB14        D12     AF4
        J302-8      PC9             PC9         D11
        J302-9      PH10            PH10        D10
        J302-10     PB1             PB1         D09
        J302-11     I2C1_SCL        PB6         D08     AF4
        J302-12     I2C1_SDA        PB7         D07     AF4
        J302-13     PB0             PB0         D06
        J302-14     PC7             PC7         D05

        LED_B       PA0
        LED_G       PA1
        LED_R       PA2
    */
}
