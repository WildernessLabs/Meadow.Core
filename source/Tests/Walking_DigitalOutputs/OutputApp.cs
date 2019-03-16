using System;
using System.Threading;
using System.Collections.Generic;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Diagnostics;

namespace Walking_DigitalOutputs
{
    public class OutputApp : AppBase<F7Micro, OutputApp>
    {
        IList<IDigitalOutputPort> _outs = new List<IDigitalOutputPort>();
        IList<string> _outChans = new List<string>();

        public OutputApp()
        {
            while (true) {

                // create all our digital output ports
                this.ConfigureOutputs();
                // turn them on/off
                this.WalkOutputs();
                // tear down
                this.DisposePorts();
            }
        }

        protected void ConfigureOutputs()
        {
            foreach (var pin in Device.Pins.AllPins) {
                Console.WriteLine("Found pin: " + pin.Name);
                foreach (var channel in pin.SupportedChannels) {
                    Console.WriteLine("Contains " + channel.Name + "channel.");

                    // if it's a digital channel, create a port.
                    if(channel is IDigitalChannelInfo 
                        && !(channel is ICommunicationChannelInfo)
                        && !(channel is IPwmChannelInfo)
                        ) {
                        if (!_outChans.Contains(channel.Name)) {
                            _outs.Add(Device.CreateDigitalOutputPort(pin));
                        } else {
                            Debug.WriteLine("Cannot add pin " + pin.Name + ", as the digital channel, " + channel.Name + " exists on another pin");
                        }
                    }
                }
            }
        }

        protected void WalkOutputs()
        {
            // turn each one on for a bit.
            foreach (var port in _outs) {
                port.State = true;
                Thread.Sleep(250);
                port.State = false;
            }
        }

        // tears down all the ports. for validation only.
        protected void DisposePorts()
        {
            foreach (var port in _outs) {
                port.Dispose();
            }
            _outs.Clear();
        }

    }
}
