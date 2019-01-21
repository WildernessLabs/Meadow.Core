using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading analog input.
    /// 
    /// Note: this class is not implemented.
    /// </summary>
    public class AnalogInputPort : AnalogInputPortBase
    {
        public override float RawValue => throw new NotImplementedException();

        public override float Voltage => throw new NotImplementedException();

        public AnalogInputPort(IAnalogPin pin) : base (pin)
        {
        }

    }
}