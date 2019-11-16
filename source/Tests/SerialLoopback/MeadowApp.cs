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

            Console.WriteLine("Getting ports...");
            var s = SerialPort.GetPortNames();
            Console.WriteLine($"Ports:\n\t{string.Join(' ', s)}");

            var portName = "ttyS1";

            Console.WriteLine($"Using '{portName}'...");

            var port = Device.CreateSerialPort(Device.SerialPortNames.Com4, 115200);

            Console.WriteLine("\tCreated");
            port.ReadTimeout = Timeout.Infinite;
            port.Open();

            var testWithEvents = false;

            if (testWithEvents)
            {
                EventTestByTokenDelimter(port);
            }
            else
            {
                PollTestByTokenDelimter(port);
            }
        }

        private string[] TestSentences = {
            "Hellow Meadow!",
            "Ground control to Major Tom.",
            "Those evil-natured robots, they're programmed to destroy us",
            "Life, it seems, will fade away. Drifting further every day. Getting lost within myself, nothing matters, no one else.",
            "It's gonna be a bright, bright, sun-shiny day!"
        };

        private static char DelimiterToken = '\n';
        private static byte DelimiterByte = Convert.ToByte(DelimiterToken);


        void EventTestByTokenDelimter(ISerialPort port)
        {
            // clear out anything already in the port buffer
            Thread.Sleep(50);
            port.DiscardInBuffer();

            port.DataReceived += OnSerialDataReceived;

            while (true)
            {
                foreach (var sentence in TestSentences)
                {
                    var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                    var written = port.Write(dataToWrite);
                    Console.WriteLine($"Wrote {written} bytes");

                    Thread.Sleep(2000);
                }
            }
        }
    
        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Serial data received");

            var port = sender as SerialPort;

            // wait for all data (everything up to a token) to be read to the input buffer
            var read = port.ReadToToken(DelimiterByte);
            while (read.Length > 0)
            {
                // don't show the token
                Console.WriteLine($"Read {read.Length} bytes: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");
            }
        }

        void PollTestByTokenDelimter(ISerialPort port)
        {
            // clear out anything already in the port buffer
            Thread.Sleep(50);
            port.DiscardInBuffer();

            while (true)
            {
                Console.WriteLine("Writing data...");
                foreach(var sentence in TestSentences)
                {
                    var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                    var written = port.Write(dataToWrite);
                    Console.WriteLine($"Wrote {written} bytes");

                    // wait for all data (everything up to a token) to be read to the input buffer
                    var read = port.ReadToToken(DelimiterByte);
                    while(read.Length == 0)
                    {
                        Thread.Sleep(50);
                        read = port.ReadToToken(DelimiterByte);
                    }

                    // don't show the token
                    Console.WriteLine($"Read {read.Length} bytes: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");

                    Thread.Sleep(2000);
                }
            }
        }
    }
}
