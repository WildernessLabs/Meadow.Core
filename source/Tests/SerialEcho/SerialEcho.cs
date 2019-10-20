using Meadow;
using Meadow.Devices;
using System;

namespace SerialEcho
{
    public class SerialEcho : App<F7Micro, SerialEcho>
    {
        public SerialEcho()
        {
            Console.WriteLine("+SerialEcho");

            Run();
        }

        void Run()
        {
            Console.WriteLine("Avaliable ports:");
            var ports = F7Serial.GetAvailablePorts();
            foreach (var p in ports)
            {
                Console.WriteLine($"\t{p}");
            }
            Console.WriteLine("Opening...");
        }
    }
}
