using Meadow.Devices;
using Meadow.Gateways;

namespace Meadow
{
    public interface IF7MeadowDevice : IMeadowDevice
    {
        IBluetoothAdapter? BluetoothAdapter { get; }
        ICoprocessor? Coprocessor { get; }
    }
}
