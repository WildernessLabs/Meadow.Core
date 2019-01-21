using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a pin that can is connected to a Pulse-Width-Modulation (PWM) 
    /// channel on the Meadow device.
    /// </summary>
    public class PwmPin : PwmPinBase
    {
        public PwmPin (string name, object key, double minimumFrequency = 0,
                       double maximumFrequency = 100000)
            : base(name, key, minimumFrequency, maximumFrequency)
        {

        }

        public override IGPIOManager GPIOManager { get; internal set; }
    }
}
