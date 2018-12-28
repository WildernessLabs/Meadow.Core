using System;
using System.Collections.Generic;
using Meadow.Core.Interop;
using Meadow.Hardware;
using static Meadow.Core.Interop.Interop.Nuttx;

namespace Meadow.Devices
{
    public class F7GPIOManager : IGPIOManager
    {
        private const string GPIODriverName = "/dev/gpio";

        private object _cacheLock = new object();
        private Dictionary<string, PinDesignator> _designatorCache = new Dictionary<string, PinDesignator>();

        private IntPtr DriverHandle { get; }

        internal F7GPIOManager()
        {
            DriverHandle = Interop.Nuttx.open(GPIODriverName, Interop.Nuttx.DriverFlags.ReadOnly);
        }

        void IGPIOManager.SetDiscrete(IDigitalPin pin, bool value)
        {
            // generate a PinState for the desired pin and value
            var state = new GPIOPinState();
            state.PinDesignator = GetPinDesignator(pin);
            state.State = value;

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
        }

        public bool GetDiscrete(IDigitalPin pin)
        {
            throw new NotImplementedException();
        }
    }
}
