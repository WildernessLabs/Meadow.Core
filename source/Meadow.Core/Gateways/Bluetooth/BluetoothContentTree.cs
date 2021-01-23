using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    public class BluetoothContentTree
    {
        ObservableCollection<IService> Services = new ObservableCollection<IService>();

        public BluetoothContentTree()
        {
        }
    }
}
