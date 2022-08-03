using Meadow.Devices;
using Meadow.Gateways;

namespace Meadow
{
    public interface IF7MeadowDevice : IMeadowDevice
    {
        IBluetoothAdapter? BluetoothAdapter { get; }
        //        IWiFiAdapter? WiFiAdapter { get; }
        ICoprocessor? Coprocessor { get; }

        AntennaType CurrentAntenna { get; }

        void SetAntenna(AntennaType antenna, bool persist = true);
    }
}
