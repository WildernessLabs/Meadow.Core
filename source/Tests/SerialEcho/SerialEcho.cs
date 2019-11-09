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
            Console.WriteLine("Using 'ttyS1'...");
            var port = new SerialPort("ttyS1", 115200);
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

            var buffer = new byte[1024];

            while (true)
            {
                Console.WriteLine("Writing data...");
                port.Write(Encoding.ASCII.GetBytes("Hello Meadow!"));

                var read = port.Read(buffer, 0, buffer.Length);

                if (read == 0)
                {
                    Console.WriteLine($"Read {read} bytes");
                }
                else
                {
                    Console.WriteLine($"Read {read} bytes: {BitConverter.ToString(buffer, 0, read)}");
                }

                Thread.Sleep(2000);
            }
        }
    }
}
