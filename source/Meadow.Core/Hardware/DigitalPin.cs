using System;
namespace Meadow.Hardware
{
	public class DigitalPin : PinBase, IDigitalChannel
    {
        public DigitalPin(string name, uint address) : base(name, address)
        {
        }
    }
}
