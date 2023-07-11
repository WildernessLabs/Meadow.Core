using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Simulation
{
    internal class SimulatedDigitalInputPort : DigitalInputPortBase
    {
        private SimulatedPin _pin;

        public SimulatedDigitalInputPort(SimulatedPin pin, IDigitalChannelInfo channel)
            : base(pin, channel)
        {
        }

        internal void SetVoltage(Voltage voltage)
        {
            if (voltage == _pin.Voltage) return;

            _pin.Voltage = voltage;
        }

        public override bool State { get => _pin.Voltage >= SimulationEnvironment.ActiveVoltage; }

        public override ResistorMode Resistor { get; set; }
    }
}
