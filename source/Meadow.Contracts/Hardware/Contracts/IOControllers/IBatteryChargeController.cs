using System;
using Meadow.Units;

namespace Meadow.Hardware
{
    public interface IBatteryChargeController
    {
        Voltage GetBatteryLevel();
    }
}
