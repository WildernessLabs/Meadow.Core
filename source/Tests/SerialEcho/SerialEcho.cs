using Meadow;
using Meadow.Devices;
using System;
using Meadow.Hardware;
using System.Text;
using System.Threading;

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

            Console.WriteLine("Using 'ttyS1'...");
            var port = new SerialPort("ttyS1");
            Console.WriteLine("\tCreated");
            port.Open();
            if (port.IsOpen)
            {
                Console.WriteLine("\tOpened");
            }
            else
            {
                Console.WriteLine("\tFailed to Open");
            }

            while (true)
            {
                Console.WriteLine("Writing data...");
                port.Write(Encoding.ASCII.GetBytes("Hello Meadow!"));

                Thread.Sleep(2000);
            }
        }
    }
}
