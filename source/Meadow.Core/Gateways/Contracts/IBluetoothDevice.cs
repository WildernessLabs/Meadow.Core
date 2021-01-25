using System;
namespace Meadow.Gateways
{
    internal interface IBluetoothDevice
    {
        bool StartBluetoothStack(string deviceName);
    }
}
