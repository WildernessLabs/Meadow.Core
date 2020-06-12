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
        string delimiterString = "$$$";
        byte[] delimiterBytes;

        public MeadowApp()
        {
            // convert for later. 
            delimiterBytes = Encoding.ASCII.GetBytes(delimiterString);

            Console.WriteLine("SerialMessagePort_Test");
            Console.WriteLine($"Using '{Device.SerialPortNames.Com1.FriendlyName}'...");
            Console.WriteLine($"delimiter:{delimiterString}");

            TestSuffixDelimiter();
        }

        /// <summary>
        /// Tests suffix/terminator delimited message reception.
        /// </summary>
        protected void TestSuffixDelimiter()
        {
            // TEST PARAM
            // whether or not to return the message with the tokens in it
            bool preseveDelimiter = false;

            // instantiate our serial port
            this.serialPort = Device.CreateSerialMessagePort(
                Device.SerialPortNames.Com1, delimiterBytes, preseveDelimiter, 115200);
            Console.WriteLine("\tCreated");

            // open the serial port
            this.serialPort.Open();
            Console.WriteLine("\tOpened");

            // wire up message received handler
            this.serialPort.MessageReceived += SerialPort_MessageReceived;

            // write to the port.
            while (true) {
                foreach (var sentence in BuildTestSentences()) {
                    //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}{DelimiterToken}");
                    var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}").Concat(delimiterBytes).ToArray();
                    //var dataToWrite = Encoding.ASCII.GetBytes($"{sentence}") + delimiter;
                    var written = this.serialPort.Write(dataToWrite);
                    Console.WriteLine($"\nWrote {written} bytes");
                    // sleep
                    Thread.Sleep(2000);
                }
            }
        }

        protected void TestPrefixDelimiter()
        {

        }



        private void SerialPort_MessageReceived(object sender, SerialMessageEventArgs e)
        {
            //Console.WriteLine($"Msg received: {e.GetMessageString()}\n");
            Console.WriteLine($"Msg recvd: {e.GetMessageString().Substring(0, 5)}...\n");
        }

        protected string[] BuildTestSentences() {
            return new string[] {
            "Hello Meadow!",
            "TrickyDouble." + delimiterString + "DoubleMessageTest",
            "Ground control to Major Tom.",
            "Those evil-natured robots, they're programmed to destroy us",
            "Life, it seems, will fade away. Drifting further every day. Getting lost within myself, nothing matters, no one else.",
            "It's gonna be a bright, bright, sun-shiny day!",
            @"Ticking away the moments that make up a dull day
Fritter and waste the hours in an offhand way.
Kicking around on a piece of ground in your home town
Waiting for someone or something to show you the way.
Tired of lying in the sunshine staying home to watch the rain.
You are young and life is long and there is time to kill today.
And then one day you find ten years have got behind you.
No one told you when to run, you missed the starting gun.
So you run and you run to catch up with the sun but it's sinking
Racing around to come up behind you again.
The sun is the same in a relative way but you're older,
Shorter of breath and one day closer to death.
Every year is getting shorter never seem to find the time.
Plans that either come to naught or half a page of scribbled lines
Hanging on in quiet desperation is the English way
The time is gone, the song is over,
Thought I'd something more to say."
                };
        }



    }
}