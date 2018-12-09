using System;

namespace Meadow.Hardware
{
    public interface IPWMChannel
    {
        double MinimumFrequency { get; }
        double MaximumFrequency { get; }
    }
}
