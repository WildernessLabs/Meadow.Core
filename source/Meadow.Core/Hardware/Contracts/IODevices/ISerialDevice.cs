using System;
namespace Meadow.Hardware
{
    public interface ISerialDevice
    {
        /// <summary>
        /// Initializes a new instance of a `ISerialPort`.
        /// When parsing text data, we recommend using the more
        /// modern, thread-safe `ISerialMessagePort`.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="baudRate">Speed, in bits per second, of the serial port.</param>
        /// <param name="parity">`Parity` enum describing what type of
        /// cyclic-redundancy-check (CRC) bit, if any, should be expected in the
        /// serial message frame. Default is `Parity.None`.</param>
        /// <param name="dataBits">Number of data bits expected in the serial
        /// message frame. Default is `8`.</param>
        /// <param name="stopBits">`StopBits` describing how many bits should be
        /// expected at the end of every character in the serial message frame.
        /// Default is `StopBits.One`.</param>
        /// <param name="readBufferSize">Size, in bytes, of the read buffer. Default
        /// is 1024.</param>
        /// <returns></returns>
        ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 1024);

    }
}
