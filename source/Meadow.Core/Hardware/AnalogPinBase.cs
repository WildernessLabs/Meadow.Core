using System;
namespace Meadow.Hardware
{
    public abstract class AnalogPinBase : PinBase, IAnalogChannel
    {
        protected AnalogPinBase(string name, uint address, byte precision) : base(name, address)
        {
            this.Precision = precision;
        }

        public byte Precision { get; protected set; }
    }
}
