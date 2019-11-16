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


            Console.WriteLine($"Using '{Device.SerialPortNames.Com4.FriendlyName}'...");

            var port = Device.CreateSerialPort(Device.SerialPortNames.Com4, 115200);

            Console.WriteLine("\tCreated");
            port.ReadTimeout = Timeout.Infinite;
            port.Open();

            // BUGBUG: serial events are problematic right now. known threading
            // issue. 
            var testWithEvents = false;

            if (testWithEvents) {
                EventTestByTokenDelimter(port);
            }
            else {
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
