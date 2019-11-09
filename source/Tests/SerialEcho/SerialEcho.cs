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
            F7Serial.GetAvailablePorts();
        }
    }
}
