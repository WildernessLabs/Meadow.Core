using System;
namespace Meadow.Hardware
{
    public class AnalogPin : PinBase, IAnalogPin
    {
        public AnalogPin(string name, uint address) : base (name, address)
        {
        }

        public byte Precision => 12;
    }
}
