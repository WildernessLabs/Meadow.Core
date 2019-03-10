using System;
using System.Threading;
using System.Collections.Generic;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Walking_DigitalOutputs
{
    public class OutputApp : AppBase<F7Micro, OutputApp>
    {
        IList<IDigitalOutputPort> _outs = new List<IDigitalOutputPort>();

        public OutputApp()
        {
            // create all our digital output ports
            this.ConfigureOutputs();
            // turn them on/off
            this.WalkOutputs();

        }

        protected void ConfigureOutputs()
        {
            foreach (var pin in Device.Pins.AllPins) {
                Console.WriteLine("Found pin: " + pin.Name);
                foreach (var channel in pin.SupportedChannels) {
                    Console.WriteLine("Contains " + channel.Name + "channel.");

                    // if it's a digital channel, create a port.
                    if(channel is IDigitalChannelInfo) {
                        _outs.Add(Device.CreateDigitalOutputPort(pin));
                    }
                }
            }
        }

        protected void WalkOutputs()
        {
            while (true) {
                // turn each one on for a bit.
                foreach (var port in _outs) {
                    port.State = true;
                    Thread.Sleep(250);
                    port.State = false;
                }
            }
        }
    }
}
