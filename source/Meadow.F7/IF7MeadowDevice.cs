using System;
using System.Threading.Tasks;
using Meadow.Devices;
using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;

namespace Meadow
{
    public interface IF7MeadowDevice :
        IMeadowDevice,
        IIOController<IF7MicroPinout>
    {
        IBluetoothAdapter? BluetoothAdapter { get; }
        IWiFiAdapter? WiFiAdapter { get; }
        ICoprocessor? Coprocessor { get; }

        Task<bool> InitCoprocessor();

        AntennaType CurrentAntenna { get; }

        void SetAntenna(AntennaType antenna, bool persist = true);

        // TODO: put on a different interface
        Temperature GetProcessorTemperature();
    }
}
