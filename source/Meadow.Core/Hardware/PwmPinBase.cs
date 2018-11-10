using System;
namespace Meadow.Hardware
{
    public abstract class PwmPinBase : DigitalPinBase, IPWMChannel
    {
        public double MinimumFrequency { get; protected set; }
        public double MaximumFrequency { get; protected set; }

        protected PwmPinBase(string name, uint address, 
                             double minimumFrequency, double maximumFrequency) 
            : base(name, address)
        {
            this.MinimumFrequency = minimumFrequency;
            this.MaximumFrequency = maximumFrequency;
        }
    }
}
