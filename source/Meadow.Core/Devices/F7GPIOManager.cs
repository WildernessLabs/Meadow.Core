using System;
using System.Collections.Generic;
using Meadow.Core.Interop;
using Meadow.Hardware;
using static Meadow.Core.Interop.Interop.Nuttx;

namespace Meadow.Devices
{
    public class F7GPIOManager : IIOController
    {
        private const string GPDDriverName = "/dev/upd";

        private object _cacheLock = new object();
        private Dictionary<string, Tuple<STM32GpioPort, int, uint>> _portPinCache = new Dictionary<string, Tuple<STM32GpioPort, int, uint>>();

        private IntPtr DriverHandle { get; }

        internal F7GPIOManager()
        {
            DriverHandle = Interop.Nuttx.open(GPDDriverName, Interop.Nuttx.DriverFlags.ReadOnly);
            if(DriverHandle == IntPtr.Zero || DriverHandle.ToInt32() == -1)
            {
                Console.Write("Failed to open UPD driver");
            }
        }

        public void Initialize()
        {
            Console.Write("Initializing GPIOs...");

            // LEDs are inverse logic - initialize to high/off
            ConfigureOutput(STM32GpioPort.PortA, 0, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, true);
            ConfigureOutput(STM32GpioPort.PortA, 1, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, true);
            ConfigureOutput(STM32GpioPort.PortA, 2, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, true);

            // these are the "unallocated" pins on the meadow
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortI, 9, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortH, 13, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortC, 6, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortB, 8, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortB, 9, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortC, 7, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortB, 0, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortB, 1, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortH, 10, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortC, 9, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortB, 14, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortB, 15, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortG, 3, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortE, 3, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);

            // these are signals that run to the ESP32
            ConfigureOutput(STM32GpioPort.PortI, 3, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortI, 2, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortD, 3, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortI, 0, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            Console.Write(".");
            ConfigureOutput(STM32GpioPort.PortI, 10, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortF, 7, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortD, 2, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);
            ConfigureOutput(STM32GpioPort.PortB, 13, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, false);

            Console.WriteLine("done");
        }

        /// <summary>
        /// Sets the value of a discrete (digital output)
        /// </summary>
        /// <param name="pin">Pin.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        void IIOController.SetDiscrete(IPin pin, bool value)
        {
            var designator = GetPortAndPin(pin);

            var register = new Interop.STM32.UpdRegisterValue();
            register.Address = designator.address + Interop.STM32.STM32_GPIO_BSRR_OFFSET;

            if(value)
            {
                register.Value = 1u << designator.pin;
            }
            else
            {
                register.Value = 1u << (designator.pin + 16);
            }

            // write the register
//            Console.WriteLine($"Writing {register.Value:X} to register: {register.Address:X}");
            var result = Interop.Nuttx.ioctl(DriverHandle, Interop.STM32.UpdIoctlFn.SetRegister, ref register);
            if (result != 0)
            {
                Console.WriteLine($"Write failed: {result}");
            }
        }

        /// <summary>
        /// Gets the value of a discrete (digital input)
        /// </summary>
        /// <returns><c>true</c>, if discrete was gotten, <c>false</c> otherwise.</returns>
        /// <param name="pin">Pin.</param>
        public bool GetDiscrete(IPin pin)
        {
            var designator = GetPortAndPin(pin);

            Interop.STM32.TryGetRegister(DriverHandle, designator.address + Interop.STM32.STM32_GPIO_IDR_OFFSET, out uint register);

            // each pin is a single bit in the register, check the bit associated with the pin number
            return (register & (1 << designator.pin)) != 0;
        }

        private (STM32GpioPort port, int pin, uint address) GetPortAndPin(IPin pin)
        {
            var key = pin.Key.ToString();
            STM32GpioPort port;
            uint address;

            lock (_portPinCache)
            {
                if (_portPinCache.ContainsKey(key))
                {
                    return (_portPinCache[key].Item1, _portPinCache[key].Item2, _portPinCache[key].Item3);
                }
                switch (key[1])
                {
                    case 'A':
                        port = STM32GpioPort.PortA;
                        address = Interop.STM32.GPIOA_BASE;
                        break;
                    case 'B':
                        port = STM32GpioPort.PortB;
                        address = Interop.STM32.GPIOB_BASE;
                        break;
                    case 'C':
                        port = STM32GpioPort.PortC;
                        address = Interop.STM32.GPIOC_BASE;
                        break;
                    case 'D':
                        port = STM32GpioPort.PortD;
                        address = Interop.STM32.GPIOD_BASE;
                        break;
                    case 'E':
                        port = STM32GpioPort.PortE;
                        address = Interop.STM32.GPIOE_BASE;
                        break;
                    case 'F':
                        port = STM32GpioPort.PortF;
                        address = Interop.STM32.GPIOF_BASE;
                        break;
                    case 'G':
                        port = STM32GpioPort.PortG;
                        address = Interop.STM32.GPIOG_BASE;
                        break;
                    case 'H':
                        port = STM32GpioPort.PortH;
                        address = Interop.STM32.GPIOH_BASE;
                        break;
                    case 'I':
                        port = STM32GpioPort.PortI;
                        address = Interop.STM32.GPIOI_BASE;
                        break;
                    case 'J':
                        port = STM32GpioPort.PortJ;
                        address = Interop.STM32.GPIOJ_BASE;
                        break;
                    case 'K':
                        port = STM32GpioPort.PortK;
                        address = Interop.STM32.GPIOK_BASE;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (int.TryParse(key.Substring(2), out int pinID))
                {
                    return (port, pinID, address);
                }

                throw new NotSupportedException();
            }
        }

        public void ConfigureOutput(IPin pin, bool initialState)
        {
            ConfigureOutput(pin, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_50MHz, STM32OutputType.PushPull, initialState);
        }

        public void ConfigureInput(IPin pin, bool glitchFilter, ResistorMode resistorMode, bool interruptEnabled)
        {
            // translate resistor mode
            STM32ResistorMode mode32;
            if (resistorMode == ResistorMode.Disabled)
            {
                mode32 = STM32ResistorMode.Float;
            }
            else if (resistorMode == ResistorMode.PullUp)
            {
                mode32 = STM32ResistorMode.PullUp;
            }
            else
            {
                mode32 = STM32ResistorMode.PullDown;
            }

            ConfigureInput(pin, mode32, interruptEnabled);
        }

        public void ConfigureAnalogInput(IPin pin)
        {
            var designator = GetPortAndPin(pin);

            // set up the GPIO register to say this is now an anlog
            // on the Meadow, all ADCs are in in ADC1
            switch (designator.port)
            {
                case STM32GpioPort.PortA:
                    ConfigureADC(designator.port, designator.pin);
                    break;
                case STM32GpioPort.PortC:
                    // channel 10 starts at C0 (see STM32F777xx pinouts, pg 68)
                    ConfigureADC(designator.port, designator.pin + 10);
                    break;
                default:
                    throw new NotSupportedException($"ADC on {pin.Key.ToString()} unknown or unsupported");
            }

            // NOTE: ADC registers will be set when the channel is actually queried
        }

        private enum STM32GpioPort
        {
            PortA,
            PortB,
            PortC,
            PortD,
            PortE,
            PortF,
            PortG,
            PortH,
            PortI,
            PortJ,
            PortK,
        }

        private enum STM32GpioMode
        {
            Input = 0,
            Output = 1,
            AlternateFunction = 2,
            Analog = 3
        }

        private enum STM32OutputType
        {
            PushPull = 0,
            OpenDrain = 1
        }

        private enum STM32ResistorMode
        {
            Float = 0,
            PullUp = 1,
            PullDown = 2
        }

        private enum STM32GPIOSpeed
        {
            Speed_2MHz = 0,
            Speed_25MHz = 1,
            Speed_50MHz = 2,
            Speed_100MHz = 3
        }

        private bool ConfigureInput(IPin pin, STM32ResistorMode resistor, bool enableInterrupts)
        {
            return ConfigureGpio(pin, STM32GpioMode.Input, resistor, STM32GPIOSpeed.Speed_2MHz, STM32OutputType.PushPull, false, enableInterrupts);
        }

        private bool ConfigureInput(STM32GpioPort port, int pin, STM32ResistorMode resistor, bool enableInterrupts)
        {
            return ConfigureGpio(port, pin, STM32GpioMode.Input, resistor, STM32GPIOSpeed.Speed_2MHz, STM32OutputType.PushPull, false, enableInterrupts);
        }

        private bool ConfigureOutput(IPin pin, STM32ResistorMode resistor, STM32GPIOSpeed speed, STM32OutputType type, bool initialState)
        {
            return ConfigureGpio(pin, STM32GpioMode.Output, resistor, speed, type, initialState, false);
        }

        private bool ConfigureOutput(STM32GpioPort port, int pin, STM32ResistorMode resistor, STM32GPIOSpeed speed, STM32OutputType type, bool initialState)
        {
            return ConfigureGpio(port, pin, STM32GpioMode.Output, resistor, speed, type, initialState, false);
        }

        private bool ConfigureADC(STM32GpioPort port, int pin)
        {
            // set up the pin for analog
            ConfigureGpio(port, pin, STM32GpioMode.Analog, STM32ResistorMode.Float, STM32GPIOSpeed.Speed_2MHz, STM32OutputType.PushPull, false, false);

            // TODO: if it was non-analog, do we need to adjust any of the ADC registers?

            return true;
        }

        private bool InitializeADC()
        {
            // do the grunt work to set up the ADC itself

            // enable the ADC1 clock - all Meadow ADCs are in ADC1
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.RCC_BASE + Interop.STM32.STM32_RCC_APB2ENR_OFFSET, 0, (1u << 8));

            // reset the ADC RCC clock - set the reset bit
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.RCC_BASE + Interop.STM32.STM32_RCC_APB2RSTR_OFFSET, 0, (1u << 8));
            // clear the reset bit
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.RCC_BASE + Interop.STM32.STM32_RCC_APB2RSTR_OFFSET, (1u << 8), 0);

            // clear the SR status register
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SR_OFFSET,
                0x1f, 0);

            // clear the CR1 control register.  This translates to:
            //  Disable all interrupts
            //  12-bit resolution
            //  Watchdog disabled
            //  Discontinuous mode disabled
            //  Auto conversion disabled
            //  scan mode disabled
            // 
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_CR1_OFFSET,
                0x7c0ffffF, 0);

            // Set up the CR2 control register.  This translates to:
            //  external trigger disabled
            //  data align right
            //  set EOC at the end of each conversion
            //  DMA disabled
            //  single conversion mode
            // 
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_CR2_OFFSET,
                0x7f7f0b03, (1 << 10));

            // Set up the SMPR1 sample time register.  This translates to:
            //  112 samle cycles for channels 10 & 11 
            // 
            Interop.STM32.UpdateRegister(DriverHandle, 
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SMPR1_OFFSET,
                0x7ffffc0, 0x2d);

            // Set up the SMPR2 sample time register.  This translates to:
            //  112 samle cycles for channels 3 & 7 
            // 
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SMPR2_OFFSET,
                0x3f1ff1ff, 0xa00a00);

            // Set up the SQR1 sequence register.  This translates to:
            //  One (1) conversion 
            // 
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SQR1_OFFSET,
                0x00ffffff, 0);

            // Set up the SQR2 sequence register.  This translates to:
            //  no conversions 7-12 
            // 
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SQR2_OFFSET,
                0x03fffffff, 0);


            // Set up the SQR3 sequence register.  This translates to:
            //  no conversions 0-6 
            // 
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SQR3_OFFSET,
                0x03fffffff, 0);
                
            // Set up the CCR common control register.  This translates to:
            //  temp sensor disabled
            //  vBAT disabled
            //  prescaler PCLK2 / 4
            //  DMA disabled
            //  independent ADCs
            // 
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SQR3_OFFSET,
                0xc0ef1f, (1 << 16));
                
            return true;
        }

        public int GetAnalogValue(IPin pin)
        {
            var designator = GetPortAndPin(pin);

            int channel;

            switch(designator.port)
            {
                case STM32GpioPort.PortA:
                    channel = designator.pin;
                    break;
                case STM32GpioPort.PortC:
                    channel = designator.pin + 10;
                    break;
                default:
                    throw new NotSupportedException($"ADC on {pin.Key.ToString()} unknown or unsupported");
            }

//            Console.WriteLine($"Starting process to get analog for channel {channel}");

            // adjust the SQR3 sequence register to tell it which channel to convert
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SQR3_OFFSET,
                0, (uint)channel);

            // enable the ADC via the CR2 register's ADON bit
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_CR2_OFFSET,
                0, 1);

//            Console.WriteLine($"Starting ADC Conversion...");

            // start a conversion via the CR2 SWSTART bit
            Interop.STM32.UpdateRegister(DriverHandle,
                Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_CR2_OFFSET,
                0, 1 << 30);

//            Console.Write($"Polling status register...");

            // poll the status register - wait for conversion complete
            var ready = false;
            do
            {
                var tick = 0;

                if (Interop.STM32.TryGetRegister(DriverHandle, Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_SR_OFFSET, out uint register_sr))
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
            if (Interop.STM32.TryGetRegister(DriverHandle, Interop.STM32.MEADOW_ADC1_BASE + Interop.STM32.ADC_DR_OFFSET, out uint register_dr))
            {
                return (int)register_dr;
            }

            throw new Exception("Conversion failed");
        }

        private bool ConfigureGpio(IPin pin, STM32GpioMode mode, STM32ResistorMode resistor, STM32GPIOSpeed speed, STM32OutputType type, bool initialState, bool enableInterrupts)
        {
            var designator = GetPortAndPin(pin);

            return ConfigureGpio(designator.port, designator.pin, mode, resistor, speed, type, initialState, enableInterrupts);
        }

        private bool ConfigureGpio(STM32GpioPort port, int pin, STM32GpioMode mode, STM32ResistorMode resistor, STM32GPIOSpeed speed, STM32OutputType type, bool initialState, bool enableInterrupts) 
        {
            int setting = 0;
            uint base_addr = 0;

            switch (port)
            {
                case STM32GpioPort.PortA: base_addr = Interop.STM32.GPIOA_BASE; break;
                case STM32GpioPort.PortB: base_addr = Interop.STM32.GPIOB_BASE; break;
                case STM32GpioPort.PortC: base_addr = Interop.STM32.GPIOC_BASE; break;
                case STM32GpioPort.PortD: base_addr = Interop.STM32.GPIOD_BASE; break;
                case STM32GpioPort.PortE: base_addr = Interop.STM32.GPIOE_BASE; break;
                case STM32GpioPort.PortF: base_addr = Interop.STM32.GPIOF_BASE; break;
                case STM32GpioPort.PortG: base_addr = Interop.STM32.GPIOG_BASE; break;
                case STM32GpioPort.PortH: base_addr = Interop.STM32.GPIOH_BASE; break;
                case STM32GpioPort.PortI: base_addr = Interop.STM32.GPIOI_BASE; break;
                case STM32GpioPort.PortJ: base_addr = Interop.STM32.GPIOJ_BASE; break;
                case STM32GpioPort.PortK: base_addr = Interop.STM32.GPIOK_BASE; break;
                default: throw new ArgumentException();
            }

            // TODO: we probably need to disable interrupts here (enter critical section)

            ////// ====== MODE ======
            // if this is an output, set the initial state
            if (mode == STM32GpioMode.Output)
            {
                var state = initialState ? 1u << pin : 1u << (16 + pin);

                Interop.STM32.SetRegister(DriverHandle, base_addr + Interop.STM32.STM32_GPIO_BSRR_OFFSET, state);
            }

            UpdateConfigRegister2Bit(base_addr + Interop.STM32.STM32_GPIO_MODER_OFFSET, (int)mode, pin);

            ////// ====== RESISTOR ======
            setting = 0;
            if (mode != STM32GpioMode.Analog)
            {
                setting = (int)resistor;
            }
            UpdateConfigRegister2Bit(base_addr + Interop.STM32.STM32_GPIO_PUPDR_OFFSET, setting, pin);


            if (mode == STM32GpioMode.AlternateFunction)
            {
                ////// ====== ALTERNATE FUNCTION ======
                // TODO:
            }

            ////// ====== SPEED ======
            setting = 0;
            if (mode == STM32GpioMode.AlternateFunction || mode == STM32GpioMode.Output)
            {
                setting = (int)speed;
            }
            UpdateConfigRegister2Bit(base_addr + Interop.STM32.STM32_GPIO_OSPEED_OFFSET, setting, pin);

            ////// ====== OUTPUT TYPE ======
            if(mode == STM32GpioMode.Output || mode == STM32GpioMode.AlternateFunction)
            {
                UpdateConfigRegister1Bit(base_addr + Interop.STM32.STM32_GPIO_OTYPER_OFFSET, (type == STM32OutputType.OpenDrain), pin);
            }
            else
            {
                UpdateConfigRegister1Bit(base_addr + Interop.STM32.STM32_GPIO_OTYPER_OFFSET, false, pin);
            }


            // TODO INTERRUPTS

            return true;
        }

        private bool UpdateConfigRegister1Bit(uint address, bool value, int pin)
        {
            if(!Interop.STM32.TryGetRegister(DriverHandle, address, out uint register))
            {
                return false;
            }

            var temp = register;
            if(value)
            {
                temp |= (1u << pin);
            }
            else
            {
                temp &= ~(1u << pin);
            }

            // write the register
            return Interop.STM32.SetRegister(DriverHandle, address, temp);
        }

        private bool UpdateConfigRegister2Bit(uint address, int value, int pin)
        {
            return Interop.STM32.UpdateRegister(DriverHandle, address, 0, (uint)(value & 3) << (pin << 1));
        }

        private bool UpdateConfigRegister2Bit_old(uint address, int value, int pin)
        {
            var register = new Interop.STM32.UpdRegisterValue();
            register.Address = address;
//            Console.WriteLine($"Reading register: {register.Address:X}");
            var result = Interop.Nuttx.ioctl(DriverHandle, Interop.STM32.UpdIoctlFn.GetRegister, ref register);
            if (result != 0)
            {
                Console.WriteLine($"Read failed: {result}");
                return false;
            }
//            Console.WriteLine($"Value: {register.Value:X}");

            var temp = register.Value;
            // mask off the bits we're interested in
            temp &= ~(3u << pin);
            // set the register bits
            temp |= (uint)value << (pin << 1);
            // write the register
            register.Value = temp;
//            Console.WriteLine($"Writing {register.Value:X} to register: {register.Address:X}");
            result = Interop.Nuttx.ioctl(DriverHandle, Interop.STM32.UpdIoctlFn.SetRegister, ref register);
            if (result != 0)
            {
                Console.WriteLine($"Write failed: {result}");
                return false;
            }
            return true;
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
