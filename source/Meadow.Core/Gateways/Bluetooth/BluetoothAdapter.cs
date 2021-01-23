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

        BluetoothContentTree ContentTree { get; } = new BluetoothContentTree();

        private IBluetoothDevice bluetoothDevice;

        private BluetoothAdapter() { }

        internal BluetoothAdapter(IBluetoothDevice device)
        {
            this.bluetoothDevice = device;

            //this.DeviceID = what?
            //this.Address = what?
            //this.Capabilities = device.BluetoothCapabilities;

            InitializeContentTree();
        }


        private void InitializeContentTree()
        {
            //TODO: setup whatever default data we want in the tree
            // e.g.:
            //IService basicService = new Service(new guid) {
            //    Name = "HonkyTonk"
            //}
            //ContentTree.Services.Add(basicService.ID, basicService);
        }

    }
}
