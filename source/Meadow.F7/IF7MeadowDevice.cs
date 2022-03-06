using Meadow.Devices;
using Meadow.Gateways;
using Meadow.Units;

namespace Meadow
{
    public interface IF7MeadowDevice : IMeadowDevice
    {
        IBluetoothAdapter? BluetoothAdapter { get; }
        IWiFiAdapter? WiFiAdapter { get; }
        ICoprocessor? Coprocessor { get; }

        AntennaType CurrentAntenna { get; }

        void SetAntenna(AntennaType antenna, bool persist = true);

        // TODO: put on a different interface
        Temperature GetProcessorTemperature();
    }
}
