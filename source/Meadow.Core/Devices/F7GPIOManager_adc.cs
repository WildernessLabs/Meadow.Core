using System;
using System.Collections.Generic;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager : IIOController
    {
        private bool _debuggingADC = true;
        private bool _initialized = false;

        public void ConfigureAnalogInput(IPin pin)
        {
#if !DEBUG
            // make sure this is off in release
            _debuggingADC = true;
#endif
            if (!_initialized)
            {
                InitializeADC();
                _initialized = true;
            }

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
            Output.WriteLineIf(_debuggingADC, $"Configuring GPIO for ADC {port}:{pin}");

            // set up the pin for analog
            ConfigureGpio(port, pin, STM32.GpioMode.Analog, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_2MHz, STM32.OutputType.PushPull, false, InterruptMode.None);

            // TODO: if it was non-analog, do we need to adjust any of the ADC registers?

            return true;
        }

        private bool InitializeADC()
        {
            Output.WriteLineIf(_debuggingADC, $"+InitializeADC");

            // do the grunt work to set up the ADC itself

            // enable the ADC1 clock - all Meadow ADCs are in ADC1
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2ENR_OFFSET, 0, (1u << 8));

            // reset the ADC RCC clock - set the reset bit
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET, 0, (1u << 8));
            // clear the reset bit
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET, (1u << 8), 0);

            // clear the SR status register
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET,
                0x1f, 0);

            // clear the CR1 control register.  This translates to:
            //  Disable all interrupts
            //  12-bit resolution
            //  Watchdog disabled
            //  Discontinuous mode disabled
            //  Auto conversion disabled
            //  scan mode disabled
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR1_OFFSET,
                0x7c0ffffF, 0);

            // Set up the CR2 control register.  This translates to:
            //  external trigger disabled
            //  data align right
            //  set EOC at the end of each conversion
            //  DMA disabled
            //  single conversion mode
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0x7f7f0b03, (1 << 10));

            // Set up the SMPR1 sample time register.  This translates to:
            //  112 sample cycles for channels 10 & 11 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR1_OFFSET,
                0x7ffffc0, 0x2d);

            // Set up the SMPR2 sample time register.  This translates to:
            //  112 sample cycles for channels 3 & 7 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR2_OFFSET,
                0x3f1ff1ff, 0xa00a00);

            // Set up the SQR1 sequence register.  This translates to:
            //  One (1) conversion 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR1_OFFSET,
                0x00ffffff, 0);

            // Set up the SQR2 sequence register.  This translates to:
            //  no conversions 7-12 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR2_OFFSET,
                0x03fffffff, 0);


            // Set up the SQR3 sequence register.  This translates to:
            //  no conversions 0-6 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                0x03fffffff, 0);

            // Set up the CCR common control register.  This translates to:
            //  temp sensor disabled
            //  vBAT disabled
            //  prescaler PCLK2 / 4
            //  DMA disabled
            //  independent ADCs
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
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

            Output.WriteLineIf(_debuggingADC, $"Starting process to get analog for channel {channel}");

            // adjust the SQR3 sequence register to tell it which channel to convert
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                0, (uint)channel);

            Output.WriteLineIf(_debuggingADC, $"SQR3 set to {channel}");

            // enable the ADC via the CR2 register's ADON bit
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0, 1);

            Output.WriteLineIf(_debuggingADC, $"CR2 ADON is set.");
            Output.WriteLineIf(_debuggingADC, $"Starting ADC Conversion...");

            // start a conversion via the CR2 SWSTART bit
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0, 1 << 30);

            Output.WriteLineIf(_debuggingADC, $"Polling status register...");

            // poll the status register - wait for conversion complete
            var ready = false;
            var tick = 0;
            do
            {
                var register_sr = UPD.GetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET);
                ready = (register_sr & (1 << 1)) != 0;

                // we need a timeout here to prevent deadlock if the SR never comes on
                if (tick++ > 200)
                {
                    // we've failed
                    Output.WriteLineIf(_debuggingADC, $"Conversion timed out");
                    return -1;
                }

                // TODO: yield
                // currently the OS hangs if I try to Sleep, so we'll spin.  BAD BAD BAD HACK
                System.Threading.Thread.Sleep(1);
            } while (!ready);

            Output.WriteLineIf(_debuggingADC, $"Conversion complete. Reading DR");

            // read the data register
            return (int)UPD.GetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_DR_OFFSET);
        }
    }
}
