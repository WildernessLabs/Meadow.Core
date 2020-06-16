using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Text;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        ISerialPort classicSerialPort;

        public MeadowApp()
        {
            Console.WriteLine("Basic_SerialListener");
            Initialize();

            ListenToSerial();

        }

        void Initialize()
        {
            this.InitSerial(Device.SerialPortNames.Com4, 9600);
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
        /// just a diagnostic method that polls the serial and outputs anything
        /// in the buffer.
        /// </summary>
        void ListenToSerial()
        {
            byte[] buffer = new byte[1024];
            int bytesToRead;

            while (true) {
                bytesToRead = classicSerialPort.BytesToRead;
                if (bytesToRead > buffer.Length) {
                    bytesToRead = buffer.Length;
                }
                int dataLength = classicSerialPort.Read(buffer, 0, bytesToRead);

                if (dataLength > 0) {
                    Console.WriteLine(ParseToString(buffer, dataLength, Encoding.ASCII));
                }
                Thread.Sleep(500);
            }

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
    }
}