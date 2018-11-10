using System;
namespace Meadow.Hardware
{
    public class AnalogPin : AnalogPinBase
    {
        public AnalogPin(string name, uint address, byte precision = 12) 
            : base(name, address, precision)
        {

        }
    }
}
