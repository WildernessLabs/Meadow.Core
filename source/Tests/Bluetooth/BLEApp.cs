using System;
using System.Collections.Generic;
using System.IO;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

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
                Device.InitBluetoothAdapter();
                Device.BluetoothAdapter.StartBluetoothStack(deviceName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
