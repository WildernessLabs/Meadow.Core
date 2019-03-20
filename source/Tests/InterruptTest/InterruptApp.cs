using System;
using System.Collections.Generic;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace InterruptTest
{
    class InterruptApp : AppBase<F7Micro, InterruptApp>
    {
        private List<IDigitalInputPort> _inputs = new List<IDigitalInputPort>();

        public InterruptApp()
        {
            ConfigureInterrupts();
        }

        private void ConfigureInterrupts()
        {
            var d0 = Device.CreateDigitalInputPort(Device.Pins.D00, interruptEnabled: true);
            d0.Changed += D0_Changed;
            _inputs.Add(d0);

            var d1 = Device.CreateDigitalInputPort(Device.Pins.D01, interruptEnabled: true);
            d1.Changed += D1_Changed;
            _inputs.Add(d1);
        }

        void D0_Changed(object sender, PortEventArgs e)
        {
            Console.WriteLine("Interrupt on D00");
        }

        void D1_Changed(object sender, PortEventArgs e)
        {
            Console.WriteLine("Interrupt on D01");
        }
    }
}
