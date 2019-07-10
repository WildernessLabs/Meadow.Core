using System;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace GpioInterrogation
{
    public class GpioApp : App<F7Micro, GpioApp>
    {
        public GpioApp()
        {
            foreach(var pin in Device.Pins.AllPins) {
                Console.WriteLine("Found pin: " + pin.Name);
                foreach (var channel in pin.SupportedChannels) {
                    Console.WriteLine("Contains a " + channel.GetType() + "channel called: " + channel.Name + ".");
                }
            }
        }

    }
}
