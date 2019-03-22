using System;
using System.Collections.Generic;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager : IIOController
    {
        public void ConfigureAnalogInput(IPin pin)
        {
            var designator = GetPortAndPin(pin);

            // set up the GPIO register to say this is now an anlog
            // on the Meadow, all ADCs are in in ADC1
            switch (designator.port)
            {
                case STM32.GpioPort.PortA:
                    ConfigureADC(designator.port, designator.pin);
                    break;
                case STM32.GpioPort.PortC:
                    // channel 10 starts at C0 (see STM32F777xx pinouts, pg 68)
                    ConfigureADC(designator.port, designator.pin + 10);
                    break;
                default:
                    throw new NotSupportedException($"ADC on {pin.Key.ToString()} unknown or unsupported");
            }

            // NOTE: ADC registers will be set when the channel is actually queried
        }

        private bool ConfigureADC(STM32.GpioPort port, int pin)
        {
            // set up the pin for analog
            ConfigureGpio(port, pin, STM32.GpioMode.Analog, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_2MHz, STM32.OutputType.PushPull, false, InterruptMode.None);

            // TODO: if it was non-analog, do we need to adjust any of the ADC registers?

            return true;
        }

        private bool InitializeADC()
        {
            // do the grunt work to set up the ADC itself

            // enable the ADC1 clock - all Meadow ADCs are in ADC1
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.RCC_BASE + STM32.STM32_RCC_APB2ENR_OFFSET, 0, (1u << 8));

            // reset the ADC RCC clock - set the reset bit
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.RCC_BASE + STM32.STM32_RCC_APB2RSTR_OFFSET, 0, (1u << 8));
            // clear the reset bit
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.RCC_BASE + STM32.STM32_RCC_APB2RSTR_OFFSET, (1u << 8), 0);

            // clear the SR status register
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET,
                0x1f, 0);

            // clear the CR1 control register.  This translates to:
            //  Disable all interrupts
            //  12-bit resolution
            //  Watchdog disabled
            //  Discontinuous mode disabled
            //  Auto conversion disabled
            //  scan mode disabled
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_CR1_OFFSET,
                0x7c0ffffF, 0);

            // Set up the CR2 control register.  This translates to:
            //  external trigger disabled
            //  data align right
            //  set EOC at the end of each conversion
            //  DMA disabled
            //  single conversion mode
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0x7f7f0b03, (1 << 10));

            // Set up the SMPR1 sample time register.  This translates to:
            //  112 samle cycles for channels 10 & 11 
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR1_OFFSET,
                0x7ffffc0, 0x2d);

            // Set up the SMPR2 sample time register.  This translates to:
            //  112 samle cycles for channels 3 & 7 
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR2_OFFSET,
                0x3f1ff1ff, 0xa00a00);

            // Set up the SQR1 sequence register.  This translates to:
            //  One (1) conversion 
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR1_OFFSET,
                0x00ffffff, 0);

            // Set up the SQR2 sequence register.  This translates to:
            //  no conversions 7-12 
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR2_OFFSET,
                0x03fffffff, 0);


            // Set up the SQR3 sequence register.  This translates to:
            //  no conversions 0-6 
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                0x03fffffff, 0);

            // Set up the CCR common control register.  This translates to:
            //  temp sensor disabled
            //  vBAT disabled
            //  prescaler PCLK2 / 4
            //  DMA disabled
            //  independent ADCs
            // 
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                0xc0ef1f, (1 << 16));

            return true;
        }

        public int GetAnalogValue(IPin pin)
        {
            var designator = GetPortAndPin(pin);

            int channel;

            switch (designator.port)
            {
                case STM32.GpioPort.PortA:
                    channel = designator.pin;
                    break;
                case STM32.GpioPort.PortC:
                    channel = designator.pin + 10;
                    break;
                default:
                    throw new NotSupportedException($"ADC on {pin.Key.ToString()} unknown or unsupported");
            }

            //            Console.WriteLine($"Starting process to get analog for channel {channel}");

            // adjust the SQR3 sequence register to tell it which channel to convert
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                0, (uint)channel);

            // enable the ADC via the CR2 register's ADON bit
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0, 1);

            //            Console.WriteLine($"Starting ADC Conversion...");

            // start a conversion via the CR2 SWSTART bit
            Interop.Nuttx.UpdateRegister(DriverHandle,
                STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0, 1 << 30);

            //            Console.Write($"Polling status register...");

            // poll the status register - wait for conversion complete
            var ready = false;
            do
            {
                var tick = 0;

                if (Interop.Nuttx.TryGetRegister(DriverHandle, STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET, out uint register_sr))
                {
                    ready = (register_sr & (1 << 1)) != 0;
                }
                else
                {
                    // this should never occur if the driver exists
                    Console.Write($"Conversion failed");
                    return -1;
                }

                // we need a timeout here to prevent deadlock if the SR never comes on
                if (tick++ > 200)
                {
                    // we've failed
                    Console.Write($"Conversion timed out");
                    return -1;
                }

                // TODO: yield
                // currently the OS hangs if I try to Sleep, so we'll spin.  BAD BAD BAD HACK
                System.Threading.Thread.Sleep(1);
            } while (!ready);

            //            Console.WriteLine($"Conversion complete");

            // read the data register
            if (Interop.Nuttx.TryGetRegister(DriverHandle, STM32.MEADOW_ADC1_BASE + STM32.ADC_DR_OFFSET, out uint register_dr))
            {
                return (int)register_dr;
            }

            throw new Exception("Conversion failed");
        }
    }
}
