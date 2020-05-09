using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    [Flags]
    internal enum DebugFeature
    {
        None,
        Startup = 1 << 0,
        Clocks = 1 << 1,
        PinInitilize = 1 << 2,
        GpioDetail = 1 << 3,
        Interrupts = 1 << 4,
        PWM = 1 << 5

    }

    internal static class Output
    {
        [Conditional("DEBUG")]
        public static void WriteIf(bool test, string value)
        {
            if (test)
            {
                Console.Write(value);
                }
        }

        [Conditional("DEBUG")]
        public static void WriteLineIf(bool test, string value)
        {
            if (test)
            {
                Console.WriteLine(value);
            }
        }

        [Conditional("DEBUG")]
        public static void Write(string value)
        {
            Console.Write(value);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }

    public partial class F7GPIOManager : IIOController
    {
        private object _cacheLock = new object();
        private bool DirectRegisterAccess { get; set; } = true;

        private Dictionary<string, Tuple<STM32.GpioPort, int, uint>> _portPinCache = new Dictionary<string, Tuple<STM32.GpioPort, int, uint>>();

        internal DebugFeature DebugFeatures { get; set; }

        internal F7GPIOManager()
        {
            DebugFeatures = DebugFeature.None;
#if DEBUG
            //Console.WriteLine($"DirectRegisterAccess = {DirectRegisterAccess}");
            // Adjust this during test and debug for your (developer)'s purposes.  The Conditional will turn it all off in a Release build.
            //DebugFeatures = DebugFeature.Startup | DebugFeature.PinInitilize | DebugFeature.GpioDetail;
            //            DebugFeatures = DebugFeature.GpioDetail;
            // p-m REMOVE THIS LINE BEFORE CHECKIN
            DebugFeatures = DebugFeature.Interrupts;
#endif
        }

        public void Initialize()
        {
            if ((DebugFeatures & DebugFeature.Interrupts) != 0)
            {
                UPD.DumpClockRegisters();
            }

            Output.WriteLineIf((DebugFeatures & DebugFeature.PinInitilize) != 0, "Initializing GPIOs...");

            // LEDs are inverse logic - initialize to high/off
            ConfigureOutput(STM32.GpioPort.PortA, 0, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, true);
            ConfigureOutput(STM32.GpioPort.PortA, 1, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, true);
            ConfigureOutput(STM32.GpioPort.PortA, 2, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, true);

            // these are the "unallocated" pins on the meadow
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortI, 9, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortH, 13, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortC, 6, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortB, 8, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortB, 9, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortC, 7, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortB, 0, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortB, 1, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortH, 10, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortC, 9, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortB, 14, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortB, 15, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortG, 3, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortE, 3, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);

            // these are signals that run to the ESP32
            ConfigureOutput(STM32.GpioPort.PortI, 3, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortI, 2, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortD, 3, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortI, 0, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            Output.WriteIf((DebugFeatures & DebugFeature.PinInitilize) != 0, ".");
            ConfigureOutput(STM32.GpioPort.PortI, 10, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortF, 7, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortD, 2, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);
            ConfigureOutput(STM32.GpioPort.PortB, 13, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, false);

            Output.WriteLineIf((DebugFeatures & DebugFeature.PinInitilize) != 0, "done");
        }

        /// <summary>
        /// Sets the value of a discrete (digital output)
        /// </summary>
        /// <param name="pin">Pin.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        void IIOController.SetDiscrete(IPin pin, bool value)
        {
            var designator = GetPortAndPin(pin);
            SetDiscrete(designator.address, designator.port, designator.pin, value);
        }

        internal unsafe void SetDiscrete(uint baseAddress, STM32.GpioPort port, int pin, bool value)
        {
            var targetAddress = baseAddress + STM32.GPIO_BSRR_OFFSET;
            var targetValue = value ? 1u << pin : 1u << (pin + 16);

            if (DirectRegisterAccess)
            {
                *(uint*)targetAddress = targetValue;
                return;
            }

            var register = new Interop.Nuttx.UpdRegisterValue
            {
                Address = targetAddress,
                Value = targetValue
            };

            // write the register
            Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0,
                $"Writing {register.Value:X} to register: {register.Address:X}");

            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SetRegister, ref register);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error);
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
            return GetDiscrete(designator.address, designator.port, designator.pin);
        }

        internal unsafe bool GetDiscrete(uint baseAddress, STM32.GpioPort port, int pin)
        {
            var targetAddress = baseAddress + STM32.GPIO_IDR_OFFSET;
            uint register;

            if (DirectRegisterAccess)
            {
                register = *(uint*)targetAddress;
            }
            else
            {
                register = UPD.GetRegister(targetAddress);
            }

            // each pin is a single bit in the register, check the bit associated with the pin number
            return (register & (1 << pin)) != 0;
        }

        private (STM32.GpioPort port, int pin, uint address) GetPortAndPin(IPin pin)
        {
            var key = pin.Key.ToString();
            STM32.GpioPort port;
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
                        port = STM32.GpioPort.PortA;
                        address = STM32.GPIOA_BASE;
                        break;
                    case 'B':
                        port = STM32.GpioPort.PortB;
                        address = STM32.GPIOB_BASE;
                        break;
                    case 'C':
                        port = STM32.GpioPort.PortC;
                        address = STM32.GPIOC_BASE;
                        break;
                    case 'D':
                        port = STM32.GpioPort.PortD;
                        address = STM32.GPIOD_BASE;
                        break;
                    case 'E':
                        port = STM32.GpioPort.PortE;
                        address = STM32.GPIOE_BASE;
                        break;
                    case 'F':
                        port = STM32.GpioPort.PortF;
                        address = STM32.GPIOF_BASE;
                        break;
                    case 'G':
                        port = STM32.GpioPort.PortG;
                        address = STM32.GPIOG_BASE;
                        break;
                    case 'H':
                        port = STM32.GpioPort.PortH;
                        address = STM32.GPIOH_BASE;
                        break;
                    case 'I':
                        port = STM32.GpioPort.PortI;
                        address = STM32.GPIOI_BASE;
                        break;
                    case 'J':
                        port = STM32.GpioPort.PortJ;
                        address = STM32.GPIOJ_BASE;
                        break;
                    case 'K':
                        port = STM32.GpioPort.PortK;
                        address = STM32.GPIOK_BASE;
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
            ConfigureOutput(pin, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, STM32.OutputType.PushPull, initialState);
        }

        public void ConfigureOutput(IPin pin, bool initialState, OutputType initialOutputType)
        {
            // translate output type from Meadow to STM32 
            STM32.OutputType stm32OutputType;
            if(initialOutputType == OutputType.PushPull)
                stm32OutputType = STM32.OutputType.PushPull;
            else
                stm32OutputType = STM32.OutputType.OpenDrain;

            ConfigureOutput(pin, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_50MHz, stm32OutputType, initialState);
        }

        private Dictionary<string, IPin> _interruptPins = new Dictionary<string, IPin>();
        
        public void ConfigureInput(
            IPin pin,
            ResistorMode resistorMode,
            InterruptMode interruptMode,
            double debounceDuration = 0.0,
            double glitchDuration = 0.0
            )
        {
            // translate resistor mode from Meadow to STM32 register bits
            STM32.ResistorMode mode32;
            if (resistorMode == ResistorMode.Disabled)
            {
                mode32 = STM32.ResistorMode.Float;
            }
            else if (resistorMode == ResistorMode.PullUp)
            {
                mode32 = STM32.ResistorMode.PullUp;
            }
            else
            {
                mode32 = STM32.ResistorMode.PullDown;
            }

            Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"Configuring {pin.Name} for resistor {resistorMode} ({(int)mode32})");

            ConfigureInput(pin, mode32, interruptMode, debounceDuration, glitchDuration);

            Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{pin.Name} configured");
        }

        private bool ConfigureInput(IPin pin, STM32.ResistorMode resistor, InterruptMode interruptMode,
            double debounceDuration, double glitchDuration)
        {
            lock (_interruptPins)
            {
                var key = (string)pin.Key;
                if (interruptMode != InterruptMode.None && !_interruptPins.ContainsKey(key))
                {
                    _interruptPins.Add(key, pin);
                }
                else if (interruptMode == InterruptMode.None && _interruptPins.ContainsKey((string)pin.Key))
                {
                    _interruptPins.Remove(key);
                }
            }
            return ConfigureGpio(pin, STM32.GpioMode.Input, resistor, STM32.GPIOSpeed.Speed_2MHz, STM32.OutputType.PushPull, 
                    false, interruptMode, debounceDuration, glitchDuration);
        }

        private bool ConfigureOutput(IPin pin, STM32.ResistorMode resistor, STM32.GPIOSpeed speed, STM32.OutputType type, bool initialState)
        {
            return ConfigureGpio(pin, STM32.GpioMode.Output, resistor, speed, type, initialState, InterruptMode.None);
        }

        internal bool ConfigureOutput(STM32.GpioPort port, int pin, STM32.ResistorMode resistor, STM32.GPIOSpeed speed, STM32.OutputType type, bool initialState)
        {
            return ConfigureGpio(port, pin, STM32.GpioMode.Output, resistor, speed, type, initialState, InterruptMode.None);
        }

        internal bool ConfigureAlternateFunction(STM32.GpioPort port, int pin, STM32.GPIOSpeed speed, STM32.OutputType type, int alternateFunction)
        {
            return ConfigureGpio(port, pin, STM32.GpioMode.AlternateFunction, STM32.ResistorMode.Float, speed, type, false, InterruptMode.None, alternateFunction);
        }

        public bool UnconfigureGpio(IPin pin)
        {
            var designator = GetPortAndPin(pin);
            return ConfigureGpio(designator.port, designator.pin, STM32.GpioMode.Input, STM32.ResistorMode.Float, STM32.GPIOSpeed.Speed_2MHz, STM32.OutputType.PushPull, false, InterruptMode.None);
        }

        private bool ConfigureGpio(IPin pin, STM32.GpioMode mode, STM32.ResistorMode resistor,
            STM32.GPIOSpeed speed, STM32.OutputType type, bool initialState, InterruptMode interruptMode,
            double debounceDuration = 0, double glitchDuration = 0)
        {
            var designator = GetPortAndPin(pin);

            return ConfigureGpio(designator.port, designator.pin, mode, resistor, speed, type, initialState, interruptMode, 0, debounceDuration, glitchDuration);
        }

        private bool ConfigureGpio(STM32.GpioPort port, int pin, STM32.GpioMode mode, STM32.ResistorMode resistor,
         STM32.GPIOSpeed speed, STM32.OutputType type, bool initialState, InterruptMode interruptMode,
         int alternateFunctionNumber = 0, double debounceDuration = 0, double glitchDuration = 0)
        {
            int setting = 0;
            uint base_addr = 0;

            switch (port)
            {
                case STM32.GpioPort.PortA: base_addr = STM32.GPIOA_BASE; break;
                case STM32.GpioPort.PortB: base_addr = STM32.GPIOB_BASE; break;
                case STM32.GpioPort.PortC: base_addr = STM32.GPIOC_BASE; break;
                case STM32.GpioPort.PortD: base_addr = STM32.GPIOD_BASE; break;
                case STM32.GpioPort.PortE: base_addr = STM32.GPIOE_BASE; break;
                case STM32.GpioPort.PortF: base_addr = STM32.GPIOF_BASE; break;
                case STM32.GpioPort.PortG: base_addr = STM32.GPIOG_BASE; break;
                case STM32.GpioPort.PortH: base_addr = STM32.GPIOH_BASE; break;
                case STM32.GpioPort.PortI: base_addr = STM32.GPIOI_BASE; break;
                case STM32.GpioPort.PortJ: base_addr = STM32.GPIOJ_BASE; break;
                case STM32.GpioPort.PortK: base_addr = STM32.GPIOK_BASE; break;
                default: throw new ArgumentException();
            }

            // TODO: we probably need to disable interrupts here (enter critical section)

            ////// ====== MODE ======
            // if this is an output, set the initial state
            if (mode == STM32.GpioMode.Output)
            {
                var state = initialState ? 1u << pin : 1u << (16 + pin);

                Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{port.ToString()} pin {pin} is an output.  Writing BSRR {state:X8}");
                UPD.SetRegister(base_addr + STM32.GPIO_BSRR_OFFSET, state);
            }

            var moder = UPD.GetRegister(base_addr + STM32.GPIO_MODER_OFFSET);
            moder &= ~(3u << (pin * 2));
            moder |= (uint)mode << (pin * 2);
            UPD.SetRegister(base_addr + STM32.GPIO_MODER_OFFSET, moder);

            ////// ====== RESISTOR ======
            setting = 0;
            if (mode != STM32.GpioMode.Analog)
            {
                SetResistorMode(base_addr, pin, (int)resistor);
                setting = (int)resistor;
            }
            else
            {
                // analogs don't need (or want!) a resistor
                SetResistorMode(base_addr, pin, 0);
            }

            if (mode == STM32.GpioMode.AlternateFunction)
            {
                Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"Configuring {port.ToString()} pin {pin} alternate function.");

                ////// ====== ALTERNATE FUNCTION ======
                var p = (int)port;
                var mask = 15u << p;
                if (p < 8)
                {
                    var bits = (uint)alternateFunctionNumber << p;
                    var afrl = UPD.GetRegister(base_addr + STM32.GPIO_AFRL_OFFSET);
                    //clear anything that was there
                    afrl &= ~mask;
                    // set the AF
                    afrl |= bits;
                    // and write it out
                    Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{port.ToString()} pin {pin} alternate function.  Writing AFRL {afrl:X8}");
                    UPD.SetRegister(base_addr + STM32.GPIO_AFRL_OFFSET, afrl);
                }
                else
                {
                    var bits = (uint)alternateFunctionNumber << (p - 8);
                    var afrh = UPD.GetRegister(base_addr + STM32.GPIO_AFRH_OFFSET);
                    //clear anything that was there
                    afrh &= ~mask;
                    // set the AF
                    afrh |= bits;
                    // and write it out
                    Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{port.ToString()} pin {pin} alternate function.  Writing AFRH {afrh:X8}");
                    UPD.SetRegister(base_addr + STM32.GPIO_AFRL_OFFSET, afrh);
                }
            }

            ////// ====== SPEED ======
            setting = 0;
            if (mode == STM32.GpioMode.AlternateFunction || mode == STM32.GpioMode.Output)
            {
                moder = UPD.GetRegister(base_addr + STM32.GPIO_OSPEED_OFFSET);
                moder &= ~(3u << (pin * 2));
                moder |= (uint)speed << (pin * 2);
                Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{port.ToString()} pin {pin} speed {speed}.  Writing OSPEED {moder:X8}");
                UPD.SetRegister(base_addr + STM32.GPIO_OSPEED_OFFSET, moder);
            }

            ////// ====== OUTPUT TYPE ======
            if (mode == STM32.GpioMode.Output || mode == STM32.GpioMode.AlternateFunction)
            {
                Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"{port.ToString()} pin {pin} open drain.");
                UpdateConfigRegister1Bit(base_addr + STM32.GPIO_OTYPER_OFFSET, (type == STM32.OutputType.OpenDrain), pin);
            }
            else
            {
                UpdateConfigRegister1Bit(base_addr + STM32.GPIO_OTYPER_OFFSET, false, pin);
            }

            ////// ====== INTERRUPTS ======
            if(mode == STM32.GpioMode.Input)
            {
                // Only input GPIOs need interrupt configuration
                WireInterrupt(port, pin, interruptMode, debounceDuration, glitchDuration);
            }

            ////// ====== REGISTER CONFIGURATION ======
            RegisterConfig(port, pin, mode, resistor, speed, type, initialState, interruptMode, alternateFunctionNumber);
            return true;
        }

        public void SetResistorMode(IPin pin, ResistorMode mode)
        {
            var designator = GetPortAndPin(pin);
            int setting = 0;
            switch (mode)
            {
                case ResistorMode.PullDown:
                    setting = (int)STM32.ResistorMode.PullDown;
                    break;
                case ResistorMode.PullUp:
                    setting = (int)STM32.ResistorMode.PullUp;
                    break;
                default:
                    setting = (int)STM32.ResistorMode.Float;
                    break;
            }
            SetResistorMode(designator.address, designator.pin, setting);
        }

        private void SetResistorMode(uint address, int pin, int mode)
        {
            // get the PUPDR register
            var addr = address + STM32.GPIO_PUPDR_OFFSET;
            var regval = UPD.GetRegister(addr);
            // turn off the 2 bits for the pin we're looking at
            regval &= (uint)~(3 << (pin << 1));
            // turn on the bits for our mode
            regval |= (uint)(mode << (pin << 1));
            // and write it out
            Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $"pin {pin} resistor mode {mode}.  Writing PUPDR {regval:X8}");
            UPD.SetRegister(addr, regval);

            if ((DebugFeatures & DebugFeature.GpioDetail) != 0)
            {
                var verify = UPD.GetRegister(addr);
                Output.WriteLine($"PUPD verify read: 0x{verify:X8}");
            }
        }

        private void UpdateConfigRegister1Bit(uint address, bool value, int pin)
        {
            var register = UPD.GetRegister(address);

            var temp = register;
            if (value)
            {
                temp |= (1u << pin);
            }
            else
            {
                temp &= ~(1u << pin);
            }

            // write the register
            UPD.SetRegister(address, temp);
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
