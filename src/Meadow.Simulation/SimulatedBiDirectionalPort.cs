using Meadow.Hardware;
using System;

namespace Meadow.Simulation
{
    internal class SimulatedBiDirectionalPort : BiDirectionalPortBase
    {
        private SimulatedPin _pin;

        public SimulatedBiDirectionalPort(IPin pin, IDigitalChannelInfo channel, bool initialState, InterruptMode interruptMode = InterruptMode.None, ResistorMode resistorMode = ResistorMode.Disabled, PortDirectionType initialDirection = PortDirectionType.Input, double debounceDuration = 0, double glitchDuration = 0, OutputType initialOutputType = OutputType.PushPull) 
            : base(pin, channel, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, initialOutputType)
        {
            _pin = pin as SimulatedPin;
            _pin.VoltageChanged += OnPinVoltageChanged;
            Direction = initialDirection;

            if (initialState)
            {
                State = InitialState;
            }
        }

        private void OnPinVoltageChanged(object sender, EventArgs e)
        {
            RaiseChangedAndNotify(new DigitalPortResult(new DigitalState(State, DateTime.Now), null));
        }

        public override bool State 
        {
            get => _pin.Voltage >= SimulationEnvironment.ActiveVoltage;
            set
            {
                if (Direction == PortDirectionType.Input) throw new Exception("Port currently set as Input");
                _pin.Voltage = value ? SimulationEnvironment.ActiveVoltage : SimulationEnvironment.InactiveVoltage;
            }
        }

        public override PortDirectionType Direction { get; set; }
        public override double DebounceDuration { get; set; }
        public override double GlitchDuration { get; set; }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
