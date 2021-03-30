using Meadow.Gateways.Bluetooth;

namespace Meadow.Gateways
{
    public interface IBluetoothAdapter
    {
        bool StartBluetoothStack(Definition configuration);
    }
}
