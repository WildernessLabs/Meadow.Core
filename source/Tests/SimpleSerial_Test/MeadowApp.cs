using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;


namespace MeadowApp
{
    /// <summary>
    /// To run these tests, create a loopback on COM4 by connecting D12 and D13.
    /// </summary>
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        ISerialPort classicSerialPort;
        Encoding currentTestEncoding = System.Text.Encoding.ASCII;

        public MeadowApp()
        {
            Console.WriteLine("SimpleSerial_Test");
            Initialize();

            Console.WriteLine("LongMessageTest - currently, not absolutely sure this works. Console.WriteLine might be clipping output.");
            Console.WriteLine("Also, test it with unicode encoding and things go sideways, are we losing a byte?? ");
            LongMessageTest().Wait();

            Console.WriteLine("BUGBUG: this test fails under specific conditions. See test for info.");
            SimpleReadWriteTest();
            Console.WriteLine("Simple read/write testing completed.");

            SerialEventTest().Wait();
            Console.WriteLine("Serial event testing completed.");
        }

        void Initialize()
        {
            this.InitSerial(Device.SerialPortNames.Com1, 115200);
            //this.InitSerial(Device.SerialPortNames.Com4, 9600);
        }

        void InitSerial(SerialPortName portName, int baud)
        {
            // instantiate our serial port
            this.classicSerialPort = Device.CreateSerialPort(portName, baud);
            Console.WriteLine("\tCreated");

            // open the serial port
            this.classicSerialPort.Open();
            Console.WriteLine("\tOpened");
        }

        /// <summary>
        /// Tests basic reading of serial in which the Write.Length == Read.Count
        /// </summary>
        void SimpleReadWriteTest()
        {
            int count = 10;
            currentTestEncoding = Encoding.Unicode;

            //Span<byte> buffer = new byte[512];
            byte[] buffer = new byte[512];

            // run the test a few times
            int dataLength = 0;
            for (int i = 0; i < 10; i++) {
                Console.WriteLine("Writing data...");
                /*dataLength =*/ this.classicSerialPort.Write(currentTestEncoding.GetBytes($"{ count * i } PRINT Hello Meadow!"));

                // give some time for the electrons to electronify
                // TODO/HACK/BUGBUG: reduce this to 100ms and weird stuff happens;
                // specifically we get the following output, and i don't know why:
                // Writing data...
                // Serial data: 0 PRINT Hello Meadow!
                // Writing data...
                // Serial data: 0 PRINT Hello Meadow!
                // Writing data...
                // Serial data: 10 PRINT Hello Meadow!
                // Writing data...
                // Serial data: 20 PRINT Hello Meadow!
                // ...
                // how is it possible that the first line is there twice, even
                // though we're clearing it out??
                Thread.Sleep(300);

                // empty it out
                dataLength = this.classicSerialPort.BytesToRead;
                this.classicSerialPort.Read(buffer, 0, dataLength);

                Console.WriteLine($"Serial data: {ParseToString(buffer, dataLength, currentTestEncoding)}");

                Thread.Sleep(300);
            }
        }

        // TODO: Someone smarter than me (bryan) needs to review this and determine
        // if my use of Span<T> is actually saving anything here.
        async Task SerialEventTest()
        {
            Console.WriteLine("SerialEventTest");

            currentTestEncoding = Encoding.Unicode;
            this.classicSerialPort.DataReceived += ProcessData;

            // send some messages
            await Task.Run(async () => {
                Console.WriteLine("Sending 8 messages of profundity.");
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("Ticking away the moments that make up a dull day,"));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("fritter and waste the hours in an offhand way."));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("Kicking around on a piece of ground in your home town,"));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("Waiting for someone or something to show you the way."));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("Tired of lying in the sunshine, staying home to watch the rain,"));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("you are young and life is long and there is time to kill today."));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("And then one day you find ten years have got behind you,"));
                await Task.Delay(100);
                this.classicSerialPort.Write(currentTestEncoding.GetBytes("No one told you when to run, you missed the starting gun."));
                await Task.Delay(100);
            });

            //weak ass Hack to wait for them all to process
            Thread.Sleep(500);

            //tear-down
            this.classicSerialPort.DataReceived -= ProcessData;
        }

        // the underlying OS provider only allows 255b messages to be sent on
        // the serial wire, so if we want to send a longer one, the `SerialPort`
        // class chunks it up
        async Task LongMessageTest()
        {
            string longMessage = @"Ticking away the moments that make up a dull day
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
Thought I'd something more to say.";

            Console.WriteLine("LongMessageTest");

            currentTestEncoding = Encoding.ASCII;

            this.classicSerialPort.DataReceived += ProcessData;

            await Task.Run(() => {
                int written = this.classicSerialPort.Write(currentTestEncoding.GetBytes(longMessage));
                Console.WriteLine($"Wrote {written} bytes");
            });
            
            //weak ass Hack to wait for them all to process
            Thread.Sleep(8000);

            //tear-down
            this.classicSerialPort.DataReceived -= ProcessData;
        }

        // anonymous method declaration so we can unwire later.
        void ProcessData(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Serial Data Received");
            byte[] buffer = new byte[512];
            int bytesToRead = classicSerialPort.BytesToRead > buffer.Length
                                ? buffer.Length
                                : classicSerialPort.BytesToRead;
            while (true) {
                int readCount = classicSerialPort.Read(buffer, 0, bytesToRead);
                Console.Write(ParseToString(buffer, readCount, currentTestEncoding));
                // if we got all the data, break the while loop, otherwise, keep going.
                if (readCount < 512) { break; }
            }
            Console.Write("\n");
        }

        /// <summary>
        /// C# compiler doesn't allow Span<T> in async methods, so can't do this
        /// inline.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected string ParseToString(byte[] buffer, int length, Encoding encoding)
        {
            Span<byte> actualData = buffer.AsSpan<byte>().Slice(0, length);
            return encoding.GetString(actualData);
        }

        void AsyncReadWaitTest()
        {

        }
    }
}