using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow.Simulation;

/// <summary>
/// A simulated implementation of the IPin interface
/// </summary>
public class SimulatedPin : Pin
{
    private Voltage _voltage;

    internal event EventHandler VoltageChanged = delegate { };

    internal SimulatedPin(IPinController controller, string name, object key, IList<IChannelInfo>? supportedChannels = null)
        : base(controller, name, key, supportedChannels)
    {
    }

    internal Voltage Voltage
    {
        get => _voltage;
        set
        {
            if (_voltage != value)
            {
                _voltage = value;
                VoltageChanged?.Invoke(this, EventArgs.Empty);
            }

        }
    }
}

