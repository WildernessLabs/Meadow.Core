using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose `ISerialMessagePort(s)`.
    /// </summary>
    public interface ISerialMessageDevice
    {
        /// <summary>
        /// Initializes a new instance of the `ISerialMessagePort` class that
        /// listens for serial messages defined byte[] message termination suffix.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="suffixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the end of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preseve the
        /// delimiter tokens when passing the message to subscribers.</param>
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
        /// is 512.</param>
        /// <returns></returns>
        ISerialMessagePort CreateSerialMessagePort(
            SerialPortName portName,
            byte[] suffixDelimiter,
            bool preserveDelimiter,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512);

        /// <summary>
        /// Initializes a new instance of the `ISerialMessagePort` class that
        /// listens for serial messages defined by a `byte[]` prefix, and a
        /// fixed length.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="messageLength">Length of the message, not including the
        /// delimiter, to be parsed out of the incoming data.</param>
        /// <param name="prefixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the beginning of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preseve the
        /// delimiter tokens when passing the message to subscribers.</param>
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
        /// is 512.</param>
        ISerialMessagePort CreateSerialMessagePort(
            SerialPortName portName,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512);
    }
}
