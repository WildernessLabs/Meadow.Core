using System;
using System.Linq;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace SerialLoopback
{
    /// <summary>
    /// Simple app that illustrates basic features of the SerialPort. To use,
    /// connect ports `D00` and `D01` with a jumper to create a serial "loopback"
    /// so that data transmitted on the TX pin is received on the RX pin.
    /// </summary>
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        ISerialPort serialPort;

        public MeadowApp()
        {
            Console.WriteLine("+SerialLoopback");

            Console.WriteLine($"Using '{Device.SerialPortNames.Com1.FriendlyName}'...");
            this.serialPort = Device.CreateSerialPort(Device.SerialPortNames.Com1, 115200);

            Console.WriteLine("\tCreated");
            this.serialPort.ReadTimeout = Timeout.Infinite;
            this.serialPort.Open();
            Console.WriteLine("\tOpened");

            //SingleEventTest(this.serialPort);

            var testWithEvents = true;

            if (testWithEvents) {
                EventTestByTokenDelimter(this.serialPort);
            } else {
                PollTestByTokenDelimter(this.serialPort);
            }
        }

        void SimplePollingListener(ISerialPort port)
        {
            // clear out anything already in the port buffer
            Thread.Sleep(50);
            port.ClearReceiveBuffer();

            while (true) {
                // wait for all data (everything up to a token) to be read to the input buffer
                var read = port.ReadToToken(DelimiterByte);
                while (read.Length == 0) {
                    Thread.Sleep(50);
                    read = port.ReadToToken(DelimiterByte);
                }

                // don't show the token
                Console.WriteLine($"Read {read.Length} bytes: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");

                Thread.Sleep(2000);
            }
        }

        private string[] TestSentences = {
            "Hello Meadow!",
            "Ground control to Major Tom.",
            "Those evil-natured robots, they're programmed to destroy us",
            "Life, it seems, will fade away. Drifting further every day. Getting lost within myself, nothing matters, no one else.",
            "It's gonna be a bright, bright, sun-shiny day!"
        };

        private static char DelimiterToken = '\n';
        private static byte DelimiterByte = Convert.ToByte(DelimiterToken);

        string delimeterTokens = "$$BIGMONAY\r\n";  //new byte[] { Convert.ToByte('\r'), Convert.ToByte('\n') };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        void PollTestByTokenDelimter(ISerialPort port)
        {
            // clear out anything already in the port buffer
            Thread.Sleep(50);
            port.ClearReceiveBuffer();
            port.ReadTimeout = 2000;

            while (true) {

                Console.WriteLine("Writing data...");
                foreach (var sentence in TestSentences) {

                    try {
                        var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                        var written = port.Write(dataToWrite);
                        Console.WriteLine($"Wrote {written} bytes");

                        // wait for data or a timeout
                        var read = port.ReadToToken(DelimiterByte);

                        // don't show the token
                        Console.WriteLine($"Read {read.Length} bytes: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");
                    } catch (Exception ex) {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    Thread.Sleep(2000);
                }
            }
        }


        void SingleEventTest(ISerialPort port)
        {
            port.DataReceived += OnSerialDataReceived;


            var dataToWrite = Encoding.ASCII.GetBytes($"This is a test of a single event\r\n");
            var written = port.Write(dataToWrite);
            Console.WriteLine($"Wrote {written} bytes");
        }

        void EventTestByTokenDelimter(ISerialPort port)
        {
            // clear out anything already in the port buffer
            Thread.Sleep(50);
            port.ClearReceiveBuffer();

            port.DataReceived += OnSerialDataReceived;

            while (true) {
                foreach (var sentence in TestSentences) {
                    //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                    var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{delimeterTokens}");
                    var written = port.Write(dataToWrite);
                    Console.WriteLine($"Wrote {written} bytes");

                    Thread.Sleep(2000);
                }
            }
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Serial Event Received");

            var port = sender as SerialPort;

            try {
                // wait for all data (everything up to a token) to be read to the input buffer
                //var read = port.ReadToToken(DelimiterByte);
                Console.WriteLine("bout to read");
                var read = port.ReadTo(delimeterTokens, true);

                Console.WriteLine($"=== read: {read.Length} bytes.===");
                //Console.WriteLine($"msg: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");
                Console.WriteLine($"msg: {Encoding.ASCII.GetString(read, 0, read.Length)}");

            } catch (Exception ex) {
                Console.WriteLine($"Read error: {ex.Message}");
            }
        }
    }
}
