using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace SampleAnalogInputs
{
    class ADCApp : AppBase<F7Micro, ADCApp>
    {
        private IList<IAnalogInputPort> _inputs = new List<IAnalogInputPort>();

        public ADCApp()
        {
            ConfigureInputs();
            ReadADCs();
        }

        private void ConfigureInputs()
        {
            Console.WriteLine("Configuring inputs...");
            _inputs.Add(Device.CreateAnalogInputPort(Device.Pins.A02));
//            _inputs.Add(Device.CreateAnalogInputPort(Device.Pins.A03));
//            _inputs.Add(Device.CreateAnalogInputPort(Device.Pins.A04));
//            _inputs.Add(Device.CreateAnalogInputPort(Device.Pins.A05));
        }

        private void ReadADCs()
        {
            while (true)
            {
                foreach (var input in _inputs)
                {
                    var value = input.Read();
                    Console.WriteLine($"{input.Pin.Name} = {value}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
