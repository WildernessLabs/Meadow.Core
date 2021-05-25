using Meadow;
using Meadow.Devices;
using Meadow.Gateways.Bluetooth;
using System;

namespace BLETest
{
    public class BLEApp : App<F7Micro, BLEApp>
    {
        public BLEApp()
        {
            Console.WriteLine("Meadow BLE Tests");

            TurnBluetoothOn("BLE TEST");

            Console.WriteLine("Testing complete");
        }

        public void TurnBluetoothOn(string deviceName)
        {
            Console.WriteLine($"Firing up the BLE radio...");

            try
            {
                var bleDefinition = new Definition(deviceName);
                Device.InitBluetoothAdapter();
                Device.BluetoothAdapter.StartBluetoothServer(bleDefinition);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}