using System;
using System.Collections.Generic;
using Meadow.Core;
using Meadow.Hardware;
using Meadow.Units;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager : IMeadowIOController
    {
        private bool _debuggingADC = false;
        private bool _initialized = false;

        private const int DefaultAdcTimeoutMs = 200;

        public void ConfigureAnalogInput(IPin pin)
        {
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
                case STM32.GpioPort.PortC:
                    // port C uses ADC1, but still gets *configured* as port C
                    ConfigureADC(designator.port, designator.pin);
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
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2ENR_OFFSET, 0,  STM32.RCC_APB2ENR_ADC1EN);

            // reset the ADC RCC clock - set the reset bit
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET, 0, STM32.RCC_APB2RSTR_ADCRST);
            // clear the reset bit
            UPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB2RSTR_OFFSET, STM32.RCC_APB2RSTR_ADCRST, 0);

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
            // basically clear all non-reserved bits
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR1_OFFSET, 
                STM32.ADC_CR1_NON_RESERVED_MASK, 0);

            // Set up the CR2 control register.  This translates to:
            //  external trigger disabled
            //  data align right
            //  set EOC at the end of each conversion
            //  DMA disabled
            //  single conversion mode
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                STM32.ADC_CR2_NON_RESERVED_MASK, STM32.ADC_CR2_EOCS);

            // Set up the SMPR1 sample time register.  This translates to:
            //  112 sample cycles for channels 10 & 11 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR1_OFFSET,
                STM32.ADC_SMPR1_NON_RESERVED_MASK, 
                (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR1_CH10_SHIFT) | (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR1_CH11_SHIFT));

            // Set up the SMPR2 sample time register.  This translates to:
            //  112 sample cycles for channels 3, 4, 5 & 7 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SMPR2_OFFSET,
                STM32.ADC_SMPR2_NON_RESERVED_MASK,
                (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR2_CH3_SHIFT)
                | (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR2_CH4_SHIFT)
                | (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR2_CH5_SHIFT)
                | (STM32.ADC_SMPx_SAMPLING_112_CYCLES << STM32.ADC_SMPR2_CH7_SHIFT));

            // Set up the SQR1 sequence register.  This translates to:
            //  One (1) conversion 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR1_OFFSET,
                STM32.ADC_SQR1_NON_RESERVED_MASK, 0);

            // Set up the SQR2 sequence register.  This translates to:
            //  no conversions 7-12 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR2_OFFSET,
                STM32.ADC_SQR2_NON_RESERVED_MASK, 0);


            // Set up the SQR3 sequence register.  This translates to:
            //  no conversions 0-6 
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                STM32.ADC_SQR3_NON_RESERVED_MASK, 0);

            // Set up the CCR common control register.  This translates to:
            //  temp sensor disabled
            //  vBAT disabled
            //  prescaler PCLK2 / 4
            //  DMA disabled
            //  independent ADCs
            // 
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CCR_OFFSET,
                STM32.ADC_CCR_NON_RESERVED_MASK,
                STM32.ADC_CCR_PRESCALER_DIV4 << STM32.ADC_CCR_ADCPRE_SHIFT);

            // enable the ADC via the CR2 register's ADON bit
            A2DPower(true);

            Output.WriteLineIf(_debuggingADC, $"CR2 ADON is set.");

            return true;
        }

        private void A2DPower(bool on)
        {
            if (on)
            {
                UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                    0,
                    STM32.ADC_CR2_ADON);
            }
            else
            {
                // enable the ADC via the CR2 register's ADON bit
                UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                    STM32.ADC_CR2_ADON,
                    0);
            }
        }

        public Temperature GetTemperature()
        {
            if (!_initialized)
            {
                InitializeADC();
                _initialized = true;
            }

            // read the CCR
            var ccr = UPD.GetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CCR_OFFSET);

            // disable the VBAT and enable the temp sensor
            var tempCCR = (int)ccr;
            tempCCR &= ~(1 << STM32.ADC_CCR_VBATE_SHIFT);
            tempCCR |= 1 << STM32.ADC_CCR_TSVREFE_SHIFT;
            UPD.SetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CCR_OFFSET, (uint)tempCCR);

            // read channel 18
            var adc = GetAnalogValue(18, 500);

            // restore the CCR
            UPD.SetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CCR_OFFSET, ccr);

            // calculate the temp
            // voltage=(double)data/4095*3.3;
            // celsius = (voltage - 0.76) / 0.0025 + 25;
            var voltage = adc / 4095d * 3.3d;
            Temperature temperature = new Temperature((voltage - 0.76) / 0.0025 + 25, Units.Temperature.UnitType.Celsius); ;

            // TODO: I *think* the STM has factory temp calibrations set at 0x1FFF7A2E and 0x1FFF7A2E.  Try using them 
            //i.e. temp = 80d / (double)(*0x1FFF7A2E - *0x1FFF7A2C) * (adc - (double)*0x1FFF7A2C) + 30d;

            return temperature;
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
                    // PC0 and PC1 have additional functions of ADC1_IN10 and 11 (see manual 'STM32F777xx STM32F778Ax STM32F779xx')
                    channel = designator.pin + 10;
                    break;
                default:
                    throw new NotSupportedException($"ADC on {pin.Key.ToString()} unknown or unsupported");
            }

            return GetAnalogValue(channel);
        }

        private int GetAnalogValue(int channel, int timeout = DefaultAdcTimeoutMs)
        {
            Output.WriteLineIf(_debuggingADC, $"Starting process to get analog for channel {channel}");

            // adjust the SQR3 sequence register to tell it which channel to convert - we're doing 1 conversion only right now
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SQR3_OFFSET,
                STM32.ADC_SQRx_CHANNEL_MASK << STM32.ADC_SQR3_SQ1_SHIFT, // clear last channel
                (uint)channel << STM32.ADC_SQR3_SQ1_SHIFT);

            Output.WriteLineIf(_debuggingADC, $"SQR3::SQ1 set to {channel}");
            
            // make sure EOC is cleared
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET,
                STM32.ADC_SR_EOC,
                0);

            Output.WriteLineIf(_debuggingADC, $"EOC Cleared...");

            Output.WriteLineIf(_debuggingADC, $"Starting ADC Conversion...");

            // start a conversion via the CR2 SWSTART bit
            UPD.UpdateRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_CR2_OFFSET,
                0, 
                STM32.ADC_CR2_SWSTART);

            Output.WriteLineIf(_debuggingADC, $"Polling status register...");

            // poll the status register - wait for conversion complete
            bool ready;
            var tick = 0;
            do
            {
                var register_sr = UPD.GetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_SR_OFFSET);
                ready = (register_sr & STM32.ADC_SR_EOC) != 0;

                // we need a timeout here to prevent deadlock if the SR never comes on
                if (tick++ > timeout)
                {
                    // we've failed
                    Output.WriteLineIf(_debuggingADC, $"Conversion timed out");
                    return -1;
                }

                // yield
                System.Threading.Thread.Sleep(1);
            } while (!ready);

            Output.WriteLineIf(_debuggingADC, $"Conversion complete. Reading DR");

            // read the data register
            return (int)UPD.GetRegister(STM32.MEADOW_ADC1_BASE + STM32.ADC_DR_OFFSET);
        }
    }
}
