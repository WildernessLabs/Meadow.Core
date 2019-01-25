using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for an analog pin.
    /// </summary>
    public abstract class AnalogPinBase : PinBase, IAnalogPin, IAnalogChannelInfo
    {
        protected AnalogPinBase(string name, uint address, byte precision) : base(name, address)
        {
            this.Precision = precision;
        }

        public byte Precision { get; protected set; }
    }
}
