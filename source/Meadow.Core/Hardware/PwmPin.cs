using System;
namespace Meadow.Hardware
{
    public class PwmPin : PinBase, IPWMChannel, IDigitalChannel
    {
        public double MinimumFrequency { get; protected set; } = 0;
        public double MaximumFrequency { get; protected set; } = 100000;

        public PwmPin(string name, uint address) : base (name, address)
        {
        }
    }
}
