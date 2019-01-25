using System;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace GpioInterrogation
{
    public class GpioApp : AppBase<F7Micro, GpioApp>
    {
        public GpioApp()
        {
            foreach(var pin in Device.Pins.AllPins) {
                Console.WriteLine("Found pin: " + pin.Name);

                switch (pin) {
                    case IAnalogPin a:
                        Console.WriteLine("Pin is an analog pin.");
                        break;
                    case IDigitalPin d:
                        Console.WriteLine("Pin is a digtial pin.");
                        break;

                }
            }
        }

    }
}
