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
            _inputs.Add(Device.CreateDigitalInputPort(Device.Pins.D00, interruptEnabled: true));
        }
    }
}
