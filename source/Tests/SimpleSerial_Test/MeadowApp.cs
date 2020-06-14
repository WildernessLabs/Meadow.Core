using System;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;


namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        ISerialPort classicSerialPort;

        public MeadowApp()
        {
            Console.WriteLine("SimpleSerial_Test");
            Initialize();

            SimpleReadWriteTest();
            Console.WriteLine("Simple read/write testing completed.");
        }

        void Initialize()
        {
            // instantiate our serial port
            this.classicSerialPort = Device.CreateSerialPort(
                Device.SerialPortNames.Com1, 115200);
            Console.WriteLine("\tCreated");

            // open the serial port
            this.classicSerialPort.Open();
            Console.WriteLine("\tOpened");

        }

        /// <summary>
        /// Tests basic reading of serial.
        /// </summary>
        void SimpleReadWriteTest()
        {
            int count = 10;

            //Span<byte> buffer = new byte[512];
            byte[] buffer = new byte[512];

            // run the test a few times
            for (int i = 0; i < 10; i++) {

                Console.WriteLine("Writing data...");
                this.classicSerialPort.Write(Encoding.ASCII.GetBytes($"{ count * i } PRINT Hello Meadow!"));

                // empty it out
                int dataLength = this.classicSerialPort.BytesToRead;
                this.classicSerialPort.Read(buffer, 0, dataLength);

                Console.WriteLine($"Serial data: {Encoding.ASCII.GetString(buffer, 0, dataLength)}");

                Thread.Sleep(300);
            }
        }

        void SerialEventTest()
        {


        }

        void AsyncReadWaitTest()
        {

        }
    }
}