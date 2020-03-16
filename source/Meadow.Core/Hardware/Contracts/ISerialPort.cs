using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// </summary>
    public interface ISerialPort
    {
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
        int BytesToRead { get; }

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
        /// Gets the port name used for communications.
        /// </summary>
        string PortName { get; }

        /// <summary>
        /// The size, in bytes, of the receive buffer that caches message data from
        /// the attached peripheral.
        /// </summary>
        int ReceiveBufferSize { get; }

        /// <summary>
        /// The number of milliseconds before a time-out occurs when a read operation does not finish.
        /// </summary>
        /// <remarks>The time-out can be set to any value greater than zero, or set to InfiniteTimeout, in which case no time-out occurs. InfiniteTimeout is the default.</remarks>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        StopBits StopBits { get; }

        /// <summary>
        /// Indicates that data has been received through a port represented by the SerialPort object.
        /// </summary>
        event SerialDataReceivedEventHandler DataReceived;

        /// <summary>
        /// Indicates that the internal data buffer has overrun and data has been lost.
        /// </summary>
        event EventHandler BufferOverrun;

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false.
        /// </summary>
        void Close();

        /// <summary>
        /// Discards data from the serial driver's receive buffer.
        /// </summary>
        void ClearReceiveBuffer();

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Returns the next available by in the input buffer but does not consume it.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if there is no data available in the input buffer.</returns>
        int Peek();

        /// <summary>
        /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="offset">The offset in buffer at which to write the bytes.</param>
        /// <param name="count">The maximum number of bytes to read. Fewer bytes are read if count is greater than the number of bytes in the input buffer.</param>
        /// <returns>The number of bytes read.</returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Synchronously reads one byte from the SerialPort input buffer.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if the end of the stream has been read.</returns>
        int ReadByte();

        /// <summary>
        /// Reads bytes from the input buffer until a specified token is found
        /// </summary>
        /// <param name="token">The token to search for</param>
        /// <returns>All data in the buffer up to and including the specified token, if a toen exists, otherwise an empty array.</returns>
        byte[] ReadToToken(byte token);

        /// <summary>
        /// Reads bytes from the input buffer until a specified token is found
        /// </summary>
        /// <param name="token">The token to search for</param>
        /// <returns>All data in the buffer up to and including the specified token, if a toen exists, otherwise an empty array.</returns>
        byte[] ReadToToken(char token);

        string ToString();

        /// <summary>
        /// Writes data to the serial port.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Write(byte[] buffer);

        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        int Write(byte[] buffer, int offset, int count);
    }
}