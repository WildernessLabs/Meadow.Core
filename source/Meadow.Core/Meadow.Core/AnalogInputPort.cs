using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow
{
    public class AnalogInputPort : IAnalogPort
    {
        readonly Pins pin;
        private Cpu.AnalogChannel analogPin;

        public double Value { get; set; }

        public AnalogInputPort(Pins pin)
        {
            this.pin = pin;
        }

        //TODO - remove this constructor
        public AnalogInputPort(Cpu.AnalogChannel analogPin)
        {
            this.analogPin = analogPin;
        }

        public PortDirectionType DirectionType => throw new NotImplementedException();

        public SignalType SignalType => throw new NotImplementedException();
    }
}