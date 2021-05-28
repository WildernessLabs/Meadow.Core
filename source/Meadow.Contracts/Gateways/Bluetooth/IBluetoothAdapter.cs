using Meadow.Gateways.Bluetooth;

namespace Meadow.Gateways
{
    public interface IBluetoothAdapter
    {
        bool StartBluetoothServer(IDefinition configuration);
    }
}
