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

        public ObservableDictionary<Guid, IService> Services { get; } = new ObservableDictionary<Guid, IService>();

        private IBluetoothDevice bluetoothDevice;

        private BluetoothAdapter() { }

        internal BluetoothAdapter(IBluetoothDevice device)
        {
            this.bluetoothDevice = device;

            // TODO: should be Device.Name or whatever
            this.DeviceID = "MeadowF7";
            // TODO: not sure
            this.Address = 37337;
            // TODO: figure this out.
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
            //Services.Add(basicService.ID, basicService);
        }

    }
}
