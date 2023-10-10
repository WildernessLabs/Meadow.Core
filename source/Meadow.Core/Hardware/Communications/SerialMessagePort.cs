namespace Meadow.Hardware
{
    // TODO: to optimize, this really should re-implement its own serialport stuff
    // rather than using the ClassicSerialPort. That way we don't maintain two
    // buffers; one in the underlying port, and one in this.

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// 
    /// Has a streamlined API over class SerialPort that deals in messages.
    ///
    /// This is a modern, asynchronous take on serial communications that is
    /// thread-safe and asynchronous in nature. This is the recommended way to
    /// use serial on Meadow for nearly all use cases.
    /// </summary>
    public class SerialMessagePort : SerialMessageProcessor, ISerialMessagePort
    {
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            get => classicSerialPort.BaudRate;
            set => classicSerialPort.BaudRate = value;
        }

        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        public string PortName { get => classicSerialPort.PortName; }

        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        public bool IsOpen { get => classicSerialPort.IsOpen; }

        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public Parity Parity { get => classicSerialPort.Parity; set => classicSerialPort.Parity = value; }

        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits { get => classicSerialPort.DataBits; set => classicSerialPort.DataBits = value; }

        /// <summary>
        /// Gets or sets the standard number of stop bits per byte.
        /// </summary>
        public StopBits StopBits { get => classicSerialPort.StopBits; set => classicSerialPort.StopBits = value; }

        /// <summary>
        /// The underlying classic serial port used for communication.
        /// </summary>
        protected ISerialPort classicSerialPort;

        /// <summary>
        /// Creates a new instance of <see cref="SerialMessagePort"/> using the specified parameters for suffix-based message processing.
        /// </summary>
        /// <param name="commsPort">The underlying serial port.</param>
        /// <param name="suffixDelimiter">The suffix delimiter for message termination.</param>
        /// <param name="preserveDelimiter">Flag indicating whether to preserve the delimiter in the received message.</param>
        /// <returns>A new instance of <see cref="SerialMessagePort"/>.</returns>
        public static SerialMessagePort From(
            ISerialPort commsPort,
            byte[] suffixDelimiter,
            bool preserveDelimiter)
        {
            return new SerialMessagePort(commsPort, suffixDelimiter, preserveDelimiter);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerialMessagePort"/> using the specified parameters for prefix-based message processing.
        /// </summary>
        /// <param name="commsPort">The underlying serial port.</param>
        /// <param name="prefixDelimiter">The prefix delimiter for message parsing.</param>
        /// <param name="preserveDelimiter">Flag indicating whether to preserve the delimiter in the received message.</param>
        /// <param name="messageLength">The expected message length (excluding the prefix delimiter).</param>
        /// <returns>A new instance of <see cref="SerialMessagePort"/>.</returns>
        public static SerialMessagePort From(
            ISerialPort commsPort,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength)
        {
            return new SerialMessagePort(commsPort, prefixDelimiter, preserveDelimiter, messageLength);
        }

        /// <summary>
        /// Initializes a new instance of the `SerialMessagePort` class that
        /// listens for serial messages defined byte[] message termination suffix.
        /// </summary>
        /// <param name="commsPort">The serial port to use.</param>
        /// <param name="suffixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the end of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preserve the
        /// delimiter tokens when passing the message to subscribers.</param>
        protected SerialMessagePort(
            ISerialPort commsPort,
            byte[] suffixDelimiter,
            bool preserveDelimiter) : base(commsPort.ReceiveBufferSize, suffixDelimiter, preserveDelimiter)
        {
            classicSerialPort = commsPort;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the `SerialMessagePort` class that
        /// listens for serial messages defined by a `byte[]` prefix, and a
        /// fixed length.
        /// </summary>
        /// <param name="commsPort">The serial port to use.</param>
        /// <param name="messageLength">Length of the message, not including the
        /// delimiter, to be parsed out of the incoming data.</param>
        /// <param name="prefixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the beginning of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preserve the
        /// delimiter tokens when passing the message to subscribers.</param>
        protected SerialMessagePort(
            ISerialPort commsPort,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength) : base(commsPort.ReceiveBufferSize, prefixDelimiter, preserveDelimiter, messageLength)
        {
            classicSerialPort = commsPort;
            Init();
        }

        /// <summary>
        /// Initializes the buffer and underlying serial port
        /// </summary>
        protected void Init()
        {
            classicSerialPort.DataReceived += SerialPortDataReceived;
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialDataType.Chars)
            {
                var data = classicSerialPort.ReadAll();
                Process(data);
            }
        }

        /// <summary>
        /// Opens a new serial port connection
        /// </summary>
        public void Open()
        {
            classicSerialPort.Open();
        }

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false
        /// </summary>
        public void Close()
        {
            classicSerialPort.Close();
        }

        /// <summary>
        /// Writes data to the serial port
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Write(byte[] buffer)
        {
            return classicSerialPort.Write(buffer);
        }

        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        public int Write(byte[] buffer, int offset, int count)
        {
            return classicSerialPort.Write(buffer, offset, count);
        }

        /// <summary>
        /// Discards all data from the receive buffer
        /// </summary>
        public void ClearReceiveBuffer()
        {
            classicSerialPort.ClearReceiveBuffer();
        }
    }
}