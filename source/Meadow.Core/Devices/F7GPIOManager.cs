using System;
using System.Collections.Generic;
using Meadow.Core.Interop;
using Meadow.Hardware;
using static Meadow.Core.Interop.Interop.Nuttx;

namespace Meadow.Devices
{
    public class F7GPIOManager : IIOController
    {
        private const string GPIODriverName = "/dev/gpio";

        private object _cacheLock = new object();
        private Dictionary<string, PinDesignator> _designatorCache = new Dictionary<string, PinDesignator>();

        private IntPtr DriverHandle { get; }

        internal F7GPIOManager()
        {
            DriverHandle = Interop.Nuttx.open(GPIODriverName, Interop.Nuttx.DriverFlags.ReadOnly);
        }

        public void ConfigureOutput(IPin pin, bool initialState)
        {
            var designator = GetPinDesignator(pin);

            // this is a safe cast, as PinDesignator and GPIOConfigFlags overlap
            var flags = (GPIOConfigFlags)designator | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;

            flags |= GPIOConfigFlags.ModeOutput;

            if (initialState)
            {
                flags |= GPIOConfigFlags.OutputInitialValueHigh;
            }
            else
            {
                flags |= GPIOConfigFlags.OutputInitialValueLow;
            }

            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref flags);
        }

        public void ConfigureInput(IPin pin, bool glitchFilter, ResistorMode resistorMode, bool interruptEnabled)
        {
            var designator = GetPinDesignator(pin);

            // this is a safe cast, as PinDesignator and GPIOConfigFlags overlap
            var flags = (GPIOConfigFlags)designator | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;

            flags |= GPIOConfigFlags.ModeInput;

            switch (resistorMode)
            {
                case ResistorMode.Disabled:
                    flags |= GPIOConfigFlags.ResistorFloat;
                    break;
                case ResistorMode.PullUp:
                    flags |= GPIOConfigFlags.ResistorPullUp;
                    break;
                case ResistorMode.PullDown:
                    flags |= GPIOConfigFlags.ResistorPullDown;
                    break;
            }

            if (interruptEnabled)
            {
                flags |= GPIOConfigFlags.InputInterruptEnable;

            }

            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref flags);
        }

        /// <summary>
        /// Sets the value out a discrete (digital output)
        /// </summary>
        /// <param name="pin">Pin.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        void IIOController.SetDiscrete(IPin pin, bool value)
        {
            // generate a PinState for the desired pin and value
            var state = new GPIOPinState();
            state.PinDesignator = GetPinDesignator(pin);

            switch(pin.Key.ToString())
            {
                case "PA0":
                case "PA1":
                case "PA2":
                    // this device uses inverted logic for the LEDs, so invert the requested value
                    state.State = !value;
                    break;
                default:
                    state.State = value;
                    break;
            }


            // and ship it to the driver
            Interop.Nuttx.ioctl(DriverHandle, Interop.Nuttx.GpioIoctlFn.Write, ref state);
        }

        private PinDesignator GetPinDesignator(IPin pin)
        {
            var key = pin.Key.ToString();

            lock (_cacheLock)
            {
                if (_designatorCache.ContainsKey(key))
                {
                    return _designatorCache[key];
                }

                // the key must be in the form 
                // P[X][Y]
                //  where
                // X == port name A - K
                // Y == 1 or 2 digit pin number. 0-15
                if (key[0] == 'P')
                {
                    PinDesignator designator;

                    switch (key[1])
                    {
                        case 'A':
                            designator = PinDesignator.PortA;
                            break;
                        case 'B':
                            designator = PinDesignator.PortB;
                            break;
                        case 'C':
                            designator = PinDesignator.PortC;
                            break;
                        case 'D':
                            designator = PinDesignator.PortD;
                            break;
                        case 'E':
                            designator = PinDesignator.PortE;
                            break;
                        case 'F':
                            designator = PinDesignator.PortF;
                            break;
                        case 'G':
                            designator = PinDesignator.PortG;
                            break;
                        case 'H':
                            designator = PinDesignator.PortH;
                            break;
                        case 'I':
                            designator = PinDesignator.PortI;
                            break;
                        case 'J':
                            designator = PinDesignator.PortJ;
                            break;
                        case 'K':
                            designator = PinDesignator.PortK;
                            break;
                        default:
                            throw new NotSupportedException();

                    }

                    if (int.TryParse(key.Substring(2), out int pinID))
                    {
                        switch (pinID)
                        {
                            case 0:
                                designator |= PinDesignator.Pin0;
                                break;
                            case 1:
                                designator |= PinDesignator.Pin1;
                                break;
                            case 2:
                                designator |= PinDesignator.Pin2;
                                break;
                            case 3:
                                designator |= PinDesignator.Pin3;
                                break;
                            case 4:
                                designator |= PinDesignator.Pin4;
                                break;
                            case 5:
                                designator |= PinDesignator.Pin5;
                                break;
                            case 6:
                                designator |= PinDesignator.Pin6;
                                break;
                            case 7:
                                designator |= PinDesignator.Pin7;
                                break;
                            case 8:
                                designator |= PinDesignator.Pin8;
                                break;
                            case 9:
                                designator |= PinDesignator.Pin9;
                                break;
                            case 10:
                                designator |= PinDesignator.Pin10;
                                break;
                            case 11:
                                designator |= PinDesignator.Pin11;
                                break;
                            case 12:
                                designator |= PinDesignator.Pin12;
                                break;
                            case 13:
                                designator |= PinDesignator.Pin13;
                                break;
                            case 14:
                                designator |= PinDesignator.Pin14;
                                break;
                            case 15:
                                designator |= PinDesignator.Pin15;
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }

                    _designatorCache.Add(key, designator);
                    return designator;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Gets the value of a discrete (digital input)
        /// </summary>
        /// <returns><c>true</c>, if discrete was gotten, <c>false</c> otherwise.</returns>
        /// <param name="pin">Pin.</param>
        public bool GetDiscrete(IPin pin)
        {
            var designator = (int)GetPinDesignator(pin);

            var result = Interop.Nuttx.ioctl(DriverHandle, Interop.Nuttx.GpioIoctlFn.Read, ref designator);

            return result != 0;
        }

        /// <summary>
        /// Initializes the device pins to their default power-up status (outputs, low and pulled down where applicable).
        /// </summary>
        public void Initialize()
        {
            // LEDs are inverse logic - initialize to high/off
            var ledBlueInit = GPIOConfigFlags.Pin0 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var ledGreenInit = GPIOConfigFlags.Pin1 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var ledRedInit = GPIOConfigFlags.Pin2 | GPIOConfigFlags.PortA | GPIOConfigFlags.OutputInitialValueHigh | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;

            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref ledBlueInit);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref ledGreenInit);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref ledRedInit);

            // all port pins seem to be inverse on this board

            // these are the "unallocated pin on the meadow
            var pi9 = GPIOConfigFlags.Pin9 | GPIOConfigFlags.PortI | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var ph13 = GPIOConfigFlags.Pin13 | GPIOConfigFlags.PortH | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pc6 = GPIOConfigFlags.Pin6 | GPIOConfigFlags.PortC | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb8 = GPIOConfigFlags.Pin8 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb9 = GPIOConfigFlags.Pin9 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pc7 = GPIOConfigFlags.Pin7 | GPIOConfigFlags.PortC | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb0 = GPIOConfigFlags.Pin0 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb7 = GPIOConfigFlags.Pin7 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb6 = GPIOConfigFlags.Pin6 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb1 = GPIOConfigFlags.Pin1 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var ph10 = GPIOConfigFlags.Pin10 | GPIOConfigFlags.PortH | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pc9 = GPIOConfigFlags.Pin9 | GPIOConfigFlags.PortC | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb14 = GPIOConfigFlags.Pin14 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pb15 = GPIOConfigFlags.Pin15 | GPIOConfigFlags.PortB | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pg3 = GPIOConfigFlags.Pin3 | GPIOConfigFlags.PortG | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;
            var pe3 = GPIOConfigFlags.Pin3 | GPIOConfigFlags.PortE | GPIOConfigFlags.OutputInitialValueLow | GPIOConfigFlags.Speed50MHz | GPIOConfigFlags.ModeOutput;

            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pi9);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref ph13);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pc6);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb8);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb9);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pc7);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb0);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb7);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb6);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb1);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref ph10);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pc9);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb14);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pb15);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pg3);
            Interop.Nuttx.ioctl(DriverHandle, GpioIoctlFn.SetConfig, ref pe3);
        }
    }

    /* ===== MEADOW GPIO PIN MAP =====
        BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   IMPLEMENTED?
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
