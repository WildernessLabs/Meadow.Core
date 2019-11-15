using System;
using System.Linq;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace SerialLoopback
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        public MeadowApp()
        {
            Console.WriteLine("+SerialLoopback");

            Run();
        }

        void Run()
        {
            Console.WriteLine("Getting ports...");
            var s = SerialPort.GetPortNames();
            Console.WriteLine($"Ports:\n\t{string.Join(' ', s)}");

            var portName = "ttyS1";

            Console.WriteLine($"Using '{portName}'...");
            var port = Device.CreateSerialPort(Device.SerialPortNames.Com4, 115200);
            //var port = new SerialPort(portName, 115200);
            Console.WriteLine("\tCreated");
            port.ReadTimeout = Timeout.Infinite;
            port.Open();
            if (port.IsOpen)
            {
                Console.WriteLine($"\tOpened {port}");
            }
            else
            {
                Console.WriteLine("\tFailed to Open");
            }

            var buffer = new byte[1024];
            port.DiscardInBuffer();

            while (true)
            {
                Console.WriteLine("Writing data...");
                var written = port.Write(Encoding.ASCII.GetBytes("Hello Meadow!"));
                Console.WriteLine($"Wrote {written} bytes");

                Console.WriteLine("Reading data...");
                var read = port.Read(buffer, 0, port.BytesToRead);

                if (read == 0)
                {
                    Console.WriteLine($"Read {read} bytes");
                }
                else
                {
                    Console.WriteLine($"Read {read} bytes: {BitConverter.ToString(buffer, 0, read)}");
                    Console.WriteLine($"Read string {Encoding.ASCII.GetString(buffer, 0, read).Replace("\r\n", "[crlf]")}");

                }

                Thread.Sleep(2000);
            }
        }
    }
}
