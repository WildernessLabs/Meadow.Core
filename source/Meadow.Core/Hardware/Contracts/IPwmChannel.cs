using System;

namespace Meadow.Hardware
{
    public interface IPwmChannel
    {
        double MinimumFrequency { get; }
        double MaximumFrequency { get; }
    }
}
