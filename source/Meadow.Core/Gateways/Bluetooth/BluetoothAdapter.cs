using System;

namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// Represents the Bluetooth communications adapter on the Meadow device.
    /// </summary>
    public class BluetoothAdapter
    {
        public string DeviceID { get; protected set; }
        public ulong Address { get; protected set; }
        public AdapterCapabilities Capabilities { get; set; }

        private IBluetoothDevice bluetoothDevice;

        private BluetoothAdapter() { }

        internal BluetoothAdapter(IBluetoothDevice device)
        {
            this.bluetoothDevice = device;

            //this.DeviceID = what?
            //this.Address = what?
            //this.Capabilities = device.BluetoothCapabilities;
        }

    }
}
