using System;
namespace Meadow.Hardware
{
    public abstract class PwmPinBase : DigitalPinBase, IPwmPin
    {
        public double MinimumFrequency { get; protected set; }
        public double MaximumFrequency { get; protected set; }

        protected PwmPinBase(string name, object key, 
                             double minimumFrequency, double maximumFrequency) 
            : base(name, key)
        {
            this.MinimumFrequency = minimumFrequency;
            this.MaximumFrequency = maximumFrequency;
        }
    }
}
