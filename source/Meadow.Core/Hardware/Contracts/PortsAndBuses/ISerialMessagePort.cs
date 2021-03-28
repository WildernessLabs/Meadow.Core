using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a serial port that provides predictable messaging.
    /// </summary>
    public interface ISerialMessagePort
    {
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        int DataBits { get; }

        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        Parity Parity { get; }

        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        StopBits StopBits { get; }

        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// The size, in bytes, of the receive buffer that caches message data from
        /// the attached peripheral.
        /// </summary>
        int ReceiveBufferSize { get; }

        /// <summary>
        /// Indicates that a message has been received through a port represented
        /// by the SerialMessagePort object.
        /// </summary>
        event EventHandler<SerialMessageData> MessageReceived;

        ///// <summary>
        ///// Indicates that the internal data buffer has overrun and data has been lost.
        ///// </summary>
        //event EventHandler BufferOverrun;

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false.
        /// </summary>
        void Close();

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Writes data to the serial port.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        //int Write(Span<byte> buffer);
        int Write(byte[] buffer);

        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        //int Write(Span<byte> buffer, int offset, int count);
        int Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Discards all data from the receive buffer.
        /// </summary>
        void ClearReceiveBuffer();
    }
}
