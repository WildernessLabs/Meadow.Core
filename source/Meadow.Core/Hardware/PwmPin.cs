using System;
namespace Meadow.Hardware
{
    public class PwmPin : PwmPinBase
    {
        public PwmPin (string name, uint address, double minimumFrequency = 0,
                       double maximumFrequency = 100000)
            : base(name, address, minimumFrequency, maximumFrequency)
        {

        }
    }
}
