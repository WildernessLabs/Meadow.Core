using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Collections.Generic;

namespace Meadow.Simulation
{
    public class SimulatedPin : Pin
    {
        private Voltage _voltage;

        internal event EventHandler VoltageChanged = delegate { };

        internal SimulatedPin(string name, object key, IList<IChannelInfo>? supportedChannels = null)
            : base(name, key, supportedChannels)
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
}

