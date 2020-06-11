using System;
using System.Linq;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware.Communications;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        SerialMessagePort serialPort;
        //byte[] delimiter = System.Text.Encoding.Unicode.GetBytes("$$BIGMONAY$$");
        byte[] delimiter = Encoding.ASCII.GetBytes("$$BIGMONAY$$");

        public MeadowApp()
        {
            Console.WriteLine("SerialMessagePort_Test");
            Console.WriteLine($"Using '{Device.SerialPortNames.Com1.FriendlyName}'...");


            Console.WriteLine($"delimiter:{Encoding.ASCII.GetString(delimiter)}");

            TestSuffixDelimiter();
        }

        void TestSuffixDelimiter()
        {
            // instantiate our serial port
            this.serialPort = Device.CreateSerialMessagePort(
                Device.SerialPortNames.Com1, delimiter, false, 115200);
            Console.WriteLine("\tCreated");

            // open the serial port
            this.serialPort.Open();
            Console.WriteLine("\tOpened");

            // wire up message received handler
            this.serialPort.MessageReceived += SerialPort_MessageReceived;

            // write to the port.
            while (true) {
                foreach (var sentence in TestSentences) {
                    //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                    var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}").Concat(delimiter).ToArray();
                    //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}") + delimiter;
                    var written = this.serialPort.Write(dataToWrite);
                    Console.WriteLine($"Wrote {written} bytes");
                    // sleep
                    Thread.Sleep(2000);
                }
            }

        }

        private void SerialPort_MessageReceived(object sender, SerialMessageEventArgs e)
        {
            Console.WriteLine($"Msg received: {e.GetMessageString()}");
        }

        private string[] TestSentences = {
            "Hello Meadow!",
            "Ground control to Major Tom.",
            "Those evil-natured robots, they're programmed to destroy us",
            "Life, it seems, will fade away. Drifting further every day. Getting lost within myself, nothing matters, no one else.",
            "It's gonna be a bright, bright, sun-shiny day!"
        };


        

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="port"></param>
        //void PollTestByTokenDelimter(ISerialPort port)
        //{
        //    // clear out anything already in the port buffer
        //    Thread.Sleep(50);
        //    port.ClearReceiveBuffer();
        //    port.ReadTimeout = 2000;

        //    while (true) {

        //        Console.WriteLine("Writing data...");
        //        foreach (var sentence in TestSentences) {

        //            try {
        //                var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
        //                var written = port.Write(dataToWrite);
        //                Console.WriteLine($"Wrote {written} bytes");

        //                // wait for data or a timeout
        //                var read = port.ReadToToken(DelimiterByte);

        //                // don't show the token
        //                Console.WriteLine($"Read {read.Length} bytes: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");
        //            } catch (Exception ex) {
        //                Console.WriteLine($"Error: {ex.Message}");
        //            }

        //            Thread.Sleep(2000);
        //        }
        //    }
        //}


        //void SingleEventTest(ISerialPort port)
        //{
        //    port.DataReceived += OnSerialDataReceived;


        //    var dataToWrite = Encoding.ASCII.GetBytes($"This is a test of a single event\r\n");
        //    var written = port.Write(dataToWrite);
        //    Console.WriteLine($"Wrote {written} bytes");
        //}

        //void EventTestByTokenDelimter(ISerialPort port)
        //{
        //    // clear out anything already in the port buffer
        //    Thread.Sleep(50);
        //    port.ClearReceiveBuffer();

        //    port.DataReceived += OnSerialDataReceived;

        //    while (true) {
        //        foreach (var sentence in TestSentences) {
        //            //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
        //            var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{delimeterTokens}");
        //            var written = port.Write(dataToWrite);
        //            Console.WriteLine($"Wrote {written} bytes");

        //            Thread.Sleep(2000);
        //        }
        //    }
        //}

        //private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    Console.WriteLine("Serial Event Received");

        //    var port = sender as SerialPort;

        //    try {
        //        // wait for all data (everything up to a token) to be read to the input buffer
        //        //var read = port.ReadToToken(DelimiterByte);
        //        Console.WriteLine("bout to read");
        //        var read = port.ReadTo(delimeterTokens, true);

        //        Console.WriteLine($"=== read: {read.Length} bytes.===");
        //        //Console.WriteLine($"msg: {Encoding.ASCII.GetString(read, 0, read.Length).TrimEnd(DelimiterToken)}");
        //        Console.WriteLine($"msg: {Encoding.ASCII.GetString(read, 0, read.Length)}");

        //    } catch (Exception ex) {
        //        Console.WriteLine($"Read error: {ex.Message}");
        //    }
        //}
    }
}