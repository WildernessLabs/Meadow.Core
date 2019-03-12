using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace ESP32ConnectionTest
{
    class MainClass
    {
        static ESP32ConnectionApp application = new ESP32ConnectionApp();

        public static void Main(string[] args)
        {
            Console.WriteLine("Testing the ESP32 UART Rx pin in output mode");

            application.Run();
        }
    }
}
