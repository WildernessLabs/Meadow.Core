using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Simulation
{
    internal class SimulatedDigitalInputPort : DigitalInputPortBase
    {
        private SimulatedPin SimPin => Pin as SimulatedPin ?? throw new System.Exception("Pin is not a SimulatedPin");

        public SimulatedDigitalInputPort(SimulatedPin pin, IDigitalChannelInfo channel)
            : base(pin, channel)
        {
        }

        internal void SetVoltage(Voltage voltage)
        {
            if (voltage == SimPin.Voltage) return;

            SimPin.Voltage = voltage;
        }

        public override bool State { get => SimPin.Voltage >= SimulationEnvironment.ActiveVoltage; }

        public override ResistorMode Resistor { get; set; }
    }
}
