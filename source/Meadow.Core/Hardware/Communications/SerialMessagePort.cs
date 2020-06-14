using System;
using System.Text;

namespace Meadow.Hardware
{

    public class SerialMessageEventArgs : EventArgs {
        public byte[] Message { get; set; } = new byte[0];

        // todo: how does this know the encoding? ASCII v Unicode v Unicode32, etc.
        public string GetMessageString()
        {
            ///return BitConverter.ToString(this.Message);
            return Encoding.ASCII.GetString(this.Message);
        }
        
    }

    // TODO: to optimize, this really should re-implement its own serialport stuff
    // rather than using the ClassicSerialPort. That way we don't maintain two
    // buffers; one in the underlying port, and one in this.

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// 
    /// Has a streamlined API over class SerialPort that deals in messages.
    ///
    /// TODO: doc better
    ///
    /// </summary>
    public class SerialMessagePort// : FilterableObservableBase<AtmosphericConditionChangeResult, AtmosphericConditions>
    {
        public event EventHandler<SerialMessageEventArgs> MessageReceived = delegate { };

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
            int baudRate,
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
            int baudRate,
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
        /// Initializes a new instance of the SerialPort class.
        /// </summary>
        /// <param name="portName">The port to use (for example, 'ttyS1').</param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readBufferSize"></param>
        protected SerialMessagePort(
            SerialPortName portName,
            byte[] suffixDelimiter,
            bool preserveDelimiter,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {
            this._messageMode = SerialMessageMode.SuffixDelimited;
            this._preserveDelimiter = preserveDelimiter;
            this._messageDelimiterTokens = suffixDelimiter;
            this._classicSerialPort = SerialPort.From(
                portName, baudRate, dataBits, parity, stopBits, readBufferSize);
            this.Init(readBufferSize);
        }

        /// <summary>
        /// Initializes a new instance of the SerialPort class.
        /// </summary>
        /// <param name="portName">The port to use (for example, 'ttyS1').</param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readBufferSize"></param>
        protected SerialMessagePort(
            SerialPortName portName,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
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
            Console.WriteLine("SerialPort_DataReceived");
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
                                    this.RaiseMessageReceivedAndNotify(new SerialMessageEventArgs() { Message = msg });
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
                                //Span<byte> msg = new byte[bytesToRemove];
                                byte[] msg = new byte[bytesToRemove];

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
                                    this.RaiseMessageReceivedAndNotify(new SerialMessageEventArgs() { Message = msg });
                                });

                                firstIndex = _readBuffer.FirstIndexOf(_messageDelimiterTokens);
                            }
                            break;
                    }
                }
            }
        }

        protected void RaiseMessageReceivedAndNotify(SerialMessageEventArgs messageResult)
        {
            MessageReceived(this, messageResult);
            //TODO: figure out the IObservable when there's no change context
            //base.NotifyObservers(messageResult);
        }

        public void Open()
        {
            this._classicSerialPort.Open();
        }

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
