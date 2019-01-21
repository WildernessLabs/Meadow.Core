using System;
namespace Meadow.Hardware
{
    public abstract class AnalogPortBase : IAnalogPort
    {
        public SignalType SignalType => SignalType.Analog;

        public abstract PortDirectionType Direction { get; }

        protected AnalogPortBase()
        {
        }
    }
}
