using System;

namespace Meadow.Hardware
{
    public class AnalogInputPort : IAnalogPort
    {
        readonly IAnalogChannel pin;
        private Cpu.AnalogChannel analogPin;

        public double Value { get; set; }

        public AnalogInputPort(IAnalogChannel pin)
        {
            this.pin = pin;
        }

        //TODO - remove this constructor
        public AnalogInputPort(Cpu.AnalogChannel analogPin)
        {
            this.analogPin = analogPin;
        }

        public PortDirectionType Direction => throw new NotImplementedException();

        public SignalType SignalType => throw new NotImplementedException();
    }
}