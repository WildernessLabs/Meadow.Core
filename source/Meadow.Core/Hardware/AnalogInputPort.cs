using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading analog input.
    /// </summary>
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