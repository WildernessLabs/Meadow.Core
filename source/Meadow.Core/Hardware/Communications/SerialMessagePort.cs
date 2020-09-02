using System;
using System.Text;

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
    public class SerialMessagePort : ISerialMessagePort// : FilterableObservableBase<AtmosphericConditionChangeResult, AtmosphericConditions>
    {
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            get => _classicSerialPort.BaudRate;
            set => _classicSerialPort.BaudRate = value;
        }
        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        public string PortName { get => _classicSerialPort.PortName; }
        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        public bool IsOpen { get => _classicSerialPort.IsOpen; }
        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public Parity Parity { get => _classicSerialPort.Parity; }
        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits { get => _classicSerialPort.DataBits; }
        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        public StopBits StopBits { get => _classicSerialPort.StopBits; }
        /// <summary>
        /// The buffer size, in bytes.
        /// </summary>
        public int ReceiveBufferSize {
            get => _classicSerialPort.ReceiveBufferSize;
        }

        /// <summary>
        /// Raised when a message, as defined in the constructor, arrives.
        /// </summary>
        public event EventHandler<SerialMessageData> MessageReceived = delegate { };

        protected SerialPort _classicSerialPort;
        protected SerialMessageMode _messageMode;
        protected byte[] _messageDelimiterTokens;
        protected int _messageLength;
        protected bool _preserveDelimiter;

        protected CircularBuffer<byte> _readBuffer;
        protected object _msgParseLock = new object();

        internal static SerialMessagePort From(
            SerialPortName portName,
            byte[] suffixDelimiter,
            bool preserveDelimiter,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096
            )
        {
            return new SerialMessagePort(
                portName, suffixDelimiter, preserveDelimiter, baudRate, dataBits,
                parity, stopBits, readBufferSize);
        }

        internal static SerialMessagePort From(
            SerialPortName portName,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096
            )
        {
            return new SerialMessagePort(
                portName, prefixDelimiter, preserveDelimiter, messageLength,
                baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        /// <summary>
        /// Initializes a new instance of the `SerialMessagePort` class that
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
        protected SerialMessagePort(
            SerialPortName portName,
            byte[] suffixDelimiter,
            bool preserveDelimiter,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512)
        {
            this._messageMode = SerialMessageMode.SuffixDelimited;
            this._preserveDelimiter = preserveDelimiter;
            this._messageDelimiterTokens = suffixDelimiter;
            this._classicSerialPort = SerialPort.From(
                portName, baudRate, dataBits, parity, stopBits, readBufferSize);
            this.Init(readBufferSize);
        }

        /// <summary>
        /// Initializes a new instance of the `SerialMessagePort` class that
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
        protected SerialMessagePort(
            SerialPortName portName,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512)
        {

            this._messageMode = SerialMessageMode.PrefixDelimited;
            this._preserveDelimiter = preserveDelimiter;
            this._messageDelimiterTokens = prefixDelimiter;
            this._messageLength = messageLength;
            this._classicSerialPort = SerialPort.From(
                portName, baudRate, dataBits, parity, stopBits, readBufferSize);
            this.Init(readBufferSize);
        }

        /// <summary>
        /// Initializes the buffer and underlying serial port
        /// </summary>
        /// <param name="readBufferSize"></param>
        protected void Init(int readBufferSize)
        {
            _readBuffer = new CircularBuffer<byte>(readBufferSize);
            this._classicSerialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine("SerialPort_DataReceived");
            // only one message processor at a time
            lock (_msgParseLock) {

                if (e.EventType == SerialDataType.Chars) {

                    // read all the available data from the underlying port
                    // HACK: note that this is where this class actually re-implementing
                    // serial port comms would be beneifical. we wouldn't have to do
                    // these additional allocations (`tempBuffer`)
                    byte[] tempBuffer = new byte[_classicSerialPort.BytesToRead];
                    _classicSerialPort.ReadAll(tempBuffer);
                    this._readBuffer.Append(tempBuffer);

                    int firstIndex;
                    switch (this._messageMode) {

                        // PREFIX DELIMITED PARSING ROUTINE
                        case SerialMessageMode.PrefixDelimited:

                            // if the buffer contains the prefix
                            firstIndex = _readBuffer.FirstIndexOf(_messageDelimiterTokens);

                            // while there are messages to Remove
                            while (firstIndex >= 0) {
                                // calculations
                                // length of the entire message that needs to be Removed
                                int totalMsgLength = _messageDelimiterTokens.Length + _messageLength;
                                // length of the message to return (depends on delimiter preservation)
                                int returnMsgLength = ( _preserveDelimiter ? totalMsgLength : _messageLength);

                                byte[] msg = new byte[returnMsgLength];

                                // throw away anything before the prefix
                                for (int i = 0; i < firstIndex; i++) {
                                    _readBuffer.Remove();
                                }

                                // presever delimiter?
                                switch (_preserveDelimiter) {
                                    case true: // if preserving, dump the whole message in
                                        for (int i = 0; i < totalMsgLength; i++) {
                                            msg[i] = _readBuffer.Remove();
                                        }
                                        break;
                                    case false:
                                        // if tossing away, throw away first part
                                        for (int i = 0; i < _messageDelimiterTokens.Length; i++) {
                                            _readBuffer.Remove();
                                        }
                                        for (int i = 0; i < returnMsgLength; i++) {
                                            msg[i] = _readBuffer.Remove();
                                        }
                                        break;
                                }

                                //todo: should this run on a new thread?
                                // it doesn't seem to return, otherwise
                                System.Threading.Tasks.Task.Run(() => {
                                    //Console.WriteLine($"raising message received, msg.length: {msg.Length}");
                                    //Console.WriteLine($"Message:{Encoding.ASCII.GetString(msg)}");
                                    this.RaiseMessageReceivedAndNotify(new SerialMessageData() { Message = msg });
                                });

                                // check if there are any left
                                firstIndex = _readBuffer.FirstIndexOf(_messageDelimiterTokens);
                            }

                            break;

                        // SUFFIX DELIMITED PARSING ROUTINE
                        case SerialMessageMode.SuffixDelimited:
                            // if the buffer contains the suffix
                            firstIndex = _readBuffer.FirstIndexOf(_messageDelimiterTokens);

                            // while there are valid messages in here (multiple
                            // messages can be in a single data event
                            while (firstIndex >= 0) {
                                var bytesToRemove = firstIndex + _messageDelimiterTokens.Length;
                                byte[] msg = new byte[(_preserveDelimiter ? bytesToRemove : (bytesToRemove - _messageDelimiterTokens.Length))];

                                // deuque the message, sans delimeter
                                for (int i = 0; i < firstIndex; i++) {
                                    msg[i] = _readBuffer.Remove();
                                }
                                // handle the delimeters. either add to msg or toss away.
                                for (int i = firstIndex; i < bytesToRemove; i++) {
                                    if (_preserveDelimiter) {
                                        msg[i] = _readBuffer.Remove();
                                    } else {
                                        _readBuffer.Remove();
                                    }
                                }

                                //todo: should this run on a new thread?
                                // it doesn't seem to return, otherwise
                                System.Threading.Tasks.Task.Run(() => {
                                    //Console.WriteLine($"raising message received, msg.length: {msg.Length}");
                                    this.RaiseMessageReceivedAndNotify(new SerialMessageData() { Message = msg });
                                });

                                firstIndex = _readBuffer.FirstIndexOf(_messageDelimiterTokens);
                            }
                            break;
                    }
                }
            }
        }

        protected void RaiseMessageReceivedAndNotify(SerialMessageData messageData)
        {
            MessageReceived(this, messageData);
            //TODO: figure out the IObservable when there's no change context
            //base.NotifyObservers(messageResult);
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        public void Open()
        {
            this._classicSerialPort.Open();
        }

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false.
        /// </summary>
        public void Close()
        {
            this._classicSerialPort.Close();
        }

        /// <summary>
        /// Writes data to the serial port.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Write(byte[] buffer)
        {
            return this._classicSerialPort.Write(buffer);
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
            return this._classicSerialPort.Write(buffer, offset, count);
        }

        /// <summary>
        /// Whether we're defining messages by prefix + length, or suffix.
        /// </summary>
        protected enum SerialMessageMode
        {
            PrefixDelimited,
            SuffixDelimited
        }

    }
}
