using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Meadow.Simulation
{
    internal class SimulatedAnalogInputPort : AnalogInputPortBase
    {
        private SimulatedPin _pin;

        public SimulatedAnalogInputPort(IPin pin, IAnalogChannelInfo channel, int sampleCount, TimeSpan sampleInterval, Voltage referenceVoltage) 
            : base(pin, channel, sampleCount, sampleInterval, referenceVoltage)
        {
            _pin = pin as SimulatedPin;
        }

        public override void Dispose()
        {
        }

        public override Task<Voltage> Read()
        {
            return Task.FromResult(_pin.Voltage);
        }

        public override void StartUpdating(TimeSpan? updateInterval)
        {
        }

        public override void StopUpdating()
        {
        }
    }
}
