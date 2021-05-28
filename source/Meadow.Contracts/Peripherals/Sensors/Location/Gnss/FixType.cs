using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Fix type / quality.
    /// </summary>
    public enum FixType
    {
        Invalid = 0,
        SPS = 1,
        DGPS = 2,
        PPS = 3,
        RealTimeKinematic = 4,
        FloatRTK = 5,
        DeadReckoning = 6,
        ManualInput = 7,
        Simulation = 8
    }
}
