using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a pin that can is connected to an anlog input and/or output
    /// channel on the Meadow device.
    /// </summary>
    public class AnalogPin : AnalogPinBase
    {
        public AnalogPin(string name, uint address, byte precision = 12) 
            : base(name, address, precision)
        {

        }
    }
}
