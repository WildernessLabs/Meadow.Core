using Meadow.Devices;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Hardware
{

    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// Preserved for legacy API compatibility. For a more modern approach, use
    /// `SerialMessagePort`.
    /// </summary>
    public abstract class SerialPortBase : IDisposable, ISerialPort
    {
        private IntPtr _driverHandle = IntPtr.Zero;
        protected bool _showSerialDebug = false;
        private CircularBuffer<byte>? _readBuffer;
        protected Thread? _readThread;
        protected int _readTimeout;
        protected int _baudRate;
        protected object _accessLock = new object();
        private int _writeTimeout;

        protected abstract void SetHardwarePortSettings(IntPtr handle);
        protected abstract IntPtr OpenHardwarePort(string portName);
        protected abstract void CloseHardwarePort(IntPtr handle);
        protected abstract int WriteHardwarePort(IntPtr handle, byte[] writeBuffer, int count);
        protected abstract int ReadHardwarePort(IntPtr handle, byte[] readBuffer, int count);

        /// <summary>
        /// Initializes a new instance of a legacy `ISerialPort`. `ISerialPort`
        /// is provided for legacy compatibility, we recommend using the more
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
        protected SerialPortBase(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {
#if !DEBUG
            // ensure this is off in release (in case a dev sets it to true and fogets during check-in
            _showSerialDebug = false;
#endif
            if (baudRate <= 0) { throw new ArgumentOutOfRangeException("Invalid baud rate"); }
            if (dataBits < 5 || dataBits > 8) { throw new ArgumentOutOfRangeException("Invalid dataBits"); }

            PortName = portName.SystemName;
            BaudRate = baudRate;
            Parity = Parity;
            DataBits = dataBits;    
            StopBits = stopBits;
            ReadTimeout = TimeSpan.FromMilliseconds(-1);
            ReceiveBufferSize = readBufferSize;
        }

        /// <summary>
        /// Indicates that data has been received through a port represented by the SerialPort object.
        /// </summary>
        public event SerialDataReceivedEventHandler DataReceived = delegate { };

        /// <summary>
        /// Indicates that the internal data buffer has overrun and data has been lost.
        /// </summary>
        public event EventHandler BufferOverrun = delegate { };

        /// <summary>
        /// Gets the port name used for communications.
        /// </summary>
        public string PortName { get; }
        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPort object.
        /// </summary>
        public bool IsOpen { get => _driverHandle != IntPtr.Zero; }
        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public Parity Parity { get; }
        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits { get; }
        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        public StopBits StopBits { get; }

        /// <summary>
        /// The time required for a a time-out to occur when a read operation does not finish.
        /// </summary>
        /// <remarks>The time-out can be set to any value greater than zero, or set to &lt;= 0, in which case no time-out occurs. InfiniteTimeout is the default.</remarks>
        public TimeSpan ReadTimeout
        {
            get => TimeSpan.FromMilliseconds(_readTimeout);
            set
            {
                _readTimeout = value.TotalMilliseconds > 0 ? (int)value.TotalMilliseconds : -1;
            }
        }

        /// <summary>
        /// The time required for a a time-out to occur when a write operation does not finish.
        /// </summary>
        /// <remarks>The time-out can be set to any value greater than zero, or set to &lt;= 0, in which case no time-out occurs. InfiniteTimeout is the default.</remarks>
        public TimeSpan WriteTimeout
        {
            get => TimeSpan.FromMilliseconds(_writeTimeout);
            set
            {
                _writeTimeout = value.TotalMilliseconds > 0 ? (int)value.TotalMilliseconds : -1;
            }
        }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
        public int BytesToRead
        {
            get => _readBuffer == null ? 0 : _readBuffer.Count;
        }

        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            get => _baudRate;
            set
            {
                if (value == BaudRate) return;
                if (value <= 0) throw new ArgumentOutOfRangeException();
                if (IsOpen) throw new IOException("You cannot change BaudRate on an Open port");

                _baudRate = value;
            }
        }

        /// <summary>
        /// Gets an array of supported baud rates
        /// </summary>
        /// <returns></returns>
        /// TODO: how about making this static?
        public int[] GetSupportedBaudRates()
        {
            return new int[]
                {
                    300,
                    600,
                    1200,
                    2400,
                    4800,
                    9600,
                    19200,
                    38400,
                    57600,
                    115200
                };
        }

        /// <summary>
        /// The buffer size, in bytes.
        /// </summary>
        public int ReceiveBufferSize
        {
            get => _readBuffer == null ? 0 : _readBuffer.MaxElements;
            private set
            {
                if (value == ReceiveBufferSize) { return; }
                // TODO: if there are events wired, we need to handle that.
                // for now this is private to prevent resizing
                _readBuffer = new CircularBuffer<byte>(value);
                _readBuffer.Overrun += OnReadBufferOverrun;
            }
        }

        private void OnReadBufferOverrun(object sender, EventArgs e)
        {
            try
            {
                BufferOverrun?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // ignore errors in the consumer's code
                Console.WriteLine($"Error in BufferOverrun handler: {ex.Message}");
            }
        }

        public override string ToString()
        {
            char p;
            switch (Parity)
            {
                case Parity.Even:
                    p = 'e';
                    break;
                case Parity.Odd:
                    p = 'o';
                    break;
                default:
                    p = 'n';
                    break;
            }

            return $"{PortName}: {BaudRate},{DataBits},{p},{(StopBits == StopBits.Two ? 2 : 1)}";
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Discards data from the serial driver's receive buffer.
        /// </summary>
        public void ClearReceiveBuffer()
        {
            if (_readBuffer != null)
            {
                _readBuffer.Clear();
                //_readBuffer.Overrun -= OnReadBufferOverrun; - Adrian, this shouldn't be needed Aug 12, 2020
            }
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        public void Open()
        {
            if (IsOpen) throw new InvalidOperationException("Port is already open");

            _driverHandle = OpenHardwarePort(PortName);
            
            SetHardwarePortSettings(_driverHandle);

            _readThread = new Thread(ReadThreadProc)
            {
                IsBackground = true,
                Name = "Serial Read Thread"
            };
            _readThread.Start();
        }

        /// <summary>
        /// Closes the port connection and sets the IsOpen property to false.
        /// </summary>
        public void Close()
        {
            if (!IsOpen) return;

            CloseHardwarePort(_driverHandle);
            
            _driverHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Writes data to the serial port.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int Write(byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        // TODO: critical section on write
        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="index">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns></returns>
        public int Write(byte[] buffer, int index, int count)
        {
            // all the checks
            if (!IsOpen) { throw new InvalidOperationException("Cannot write to a closed port"); }
            if (buffer == null) throw new ArgumentNullException();
            if (count > (buffer.Length - index)) throw new ArgumentException("Count is larger than available data");
            if (index < 0) throw new ArgumentException("Invalid offset");
            if (count == 0) return 0;

            lock (_accessLock)
            {
                int currentIndex = index;
                int totalBytesWritten = 0;
                int result = 0;
                int systemBufferMax = 255;
                int maxCount = count > systemBufferMax ? systemBufferMax : count; //if it's > 255, limit it. 
                int bytesToWriteThisLoop = maxCount;
                int bytesLeft = count;

                Output.WriteLineIf(_showSerialDebug, $"count:{count}, maxCount:{maxCount}");
                Output.WriteLineIf(_showSerialDebug, $"Starting with: {(BitConverter.ToString(buffer, 0, (buffer.Length > 6) ? 6 : buffer.Length))}");

                Timer? writeTimeoutTimer = null;

                if(WriteTimeout.TotalMilliseconds > 0)
                {
                    writeTimeoutTimer = new Timer((o) =>
                    {
                        throw new TimeoutException("Write timeout");
                    }, null, (int)WriteTimeout.TotalMilliseconds, Timeout.Infinite);
                }

                // we can only write 255 bytes at a time, so we loop 
                try
                {
                    while (totalBytesWritten < count)
                    {
                        // if there's an offset, we want to slice
                        if (currentIndex > 0)
                        {
                            Output.WriteLineIf(_showSerialDebug, $"Slicing. currentIndex:{currentIndex}, bytesLeft:{bytesLeft}, bytesToWriteThisLoop:{bytesToWriteThisLoop}");
                            Span<byte> data = buffer.AsSpan<byte>().Slice(currentIndex, bytesToWriteThisLoop);
                            result = WriteHardwarePort(_driverHandle, data.ToArray(), count);
                        }
                        else
                        {
                            result = WriteHardwarePort(_driverHandle, buffer, count);
                        }

                        // otherwise,
                        totalBytesWritten += result;

                        Output.WriteLineIf(_showSerialDebug, $"bytesActallyWrittenThisLoop: {result} totalBytesWritten: {totalBytesWritten}");

                        // recalculate the current index, including the original offset
                        currentIndex = totalBytesWritten + index;
                        bytesLeft = count - totalBytesWritten;
                        bytesToWriteThisLoop = bytesLeft > systemBufferMax ? systemBufferMax : bytesLeft;
                        Output.WriteLineIf(_showSerialDebug, $"currentIndex: {currentIndex}, bytesLeft: {bytesLeft}, bytesToWriteThisLoop:{bytesToWriteThisLoop}");
                    }
                }
                finally
                {
                    if(writeTimeoutTimer != null)
                    {
                        writeTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        writeTimeoutTimer.Dispose();
                    }
                }

                return totalBytesWritten;
            }
        }

        private void ReadThreadProc()
        {
            var readBuffer = new byte[4096];

            Output.WriteLineIf(_showSerialDebug, $"ReadThreadProc: {IsOpen}");

            while (IsOpen)
            {
                try
                {
                    var result = ReadHardwarePort(_driverHandle, readBuffer, readBuffer.Length);

                    if (result > 0)
                    {
                        Output.WriteLineIf(_showSerialDebug, $"data read");
                        Output.WriteLineIf(_showSerialDebug, $"Enqueuing {result} bytes");

                        _readBuffer?.Append(readBuffer, 0, result);

                        if (DataReceived != null)
                        {
                            // put on a worker thread, else if the handler goes into some wait, we'll never process data
                            Task.Run(() =>
                            {
                                try
                                {
                                    Output.WriteLineIf(_showSerialDebug, $"Calling event handlers");
                                    DataReceived?.Invoke(this, new SerialDataReceivedEventArgs(SerialDataType.Chars));
                                    Output.WriteLineIf(_showSerialDebug, $"event handler complete");
                                }
                                catch (Exception ex)
                                {
                                        // if the event handler throws, we don't want this to die
                                        Output.WriteLine($"Serial event handler threw: {ex.Message}");
                                }
                            });
                        }

                        // we *have* to yield to allow the receiver to process data or we'll just overrun
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"ReadThreadProc error: {ex.Message}");
                }
            }

            Output.WriteLineIf(_showSerialDebug, $"ReadThreadProc: port closed");
        }

        /// <summary>
        /// Returns the next available by in the input buffer but does not consume it.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if there is no data available in the input buffer.</returns>
        public int Peek()
        {
            if (_readBuffer == null || _readBuffer.Count == 0) return -1;
            return _readBuffer.Peek();
        }


        /// <summary>
        /// Synchronously reads one byte from the SerialPort input buffer.
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if the end of the stream has been read.</returns>
        public int ReadByte()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Cannot read from a closed port");
            }

            if (_readBuffer == null || _readBuffer.Count == 0) return -1;
            return _readBuffer.Remove();
        }


        public int ReadAll(byte[] buffer)
        {
            return ReadAll(buffer, 0);
        }

        /// <summary>
        /// Reads the entire serial port buffer into an array of bytes. Before
        /// calling, make sure that your buffer is large enough by checking
        /// `BytesToRead` property. If your buffer isn't large enough, this will
        /// leave bytes in the serial port buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int ReadAll(byte[] buffer, int index)
        {
            // checks
            if (!IsOpen) { throw new InvalidOperationException("Cannot read from a closed port"); }
            if (buffer == null) { throw new ArgumentNullException(); }
            if (index < 0) { throw new ArgumentException("Invalid index"); }

            if (_readBuffer == null) return 0;

            // capture the count
            int readCount = _readBuffer.Count;

            // check to see if there's room for the whole thing, if not, we're
            // only going to read as much as we can.
            if (readCount + index > buffer.Length)
            {
                readCount = buffer.Length - index;
            }

            // empty the serial data into the user's buffer
            _readBuffer.Remove(readCount).CopyTo(buffer, index);

            // return the count read
            return readCount;
        }


        // DON'T DELETE
        // This is preserved here as a potential solution to reading past what
        // is currently in the buffer.
        //
        // Ultimately, I don't believe this is practically possible with the
        // expected mecanics of the SerialPort.
        //
        // The issue is that in order to continue to read, we have two options,
        // 1) either sleep/spin in a loop that keeps checking the buffer to see
        //    if anything has come in, and pull it off, or:
        // 2) as i have done here, use a `ManualResetEvent` to unblock the `Read`
        //    thread when data has come in. This prevents a spinning loop.
        //
        // However, in practice, because we don't protect the underlying read
        // buffer, a user could subscribe to the `DataReceived` event and empty
        // out data in the buffer, causing the `Read` call to be useless. The only
        // way to do this would be to cancel any notifications of `DataReceived`
        // while this call was still executing. That's doable, but I think it
        // adds a lot more complexity to the archtiecture, and makes for a bad
        // API experience. As such, I think we need to restrict `Read()` to
        // data that is in the buffer. If they want an modern async model, they
        // should probalby use the SerialMessagePort
        //
        // in any case, i'm preserving this (untested) code below, in case we
        // might someday make a wrapper to SerialPort that is properly async.
        //
        // ====== CODE PRESERVED BELOW
        // we'll use this to unblock the `Read()` thread on the event of new
        // data coming in.
        // ManualResetEvent readThreadResetEvent = new ManualResetEvent(false);
        //
        //public async ValueTask<int> ReadWithResetEvent(byte[] buffer, int index, int count)
        //{
        //    // all the checks
        //    if (!IsOpen) { throw new InvalidOperationException("Cannot read from a closed port"); }
        //    if (buffer == null) { throw new ArgumentNullException(); }
        //    if (count > (buffer.Length - index)) { throw new ArgumentException("Count is larger than available buffer size"); }
        //    if (index < 0) { throw new ArgumentException("Invalid offset"); }
        //    if (count == 0) { return 0; }

        //    // if they want less than or as much data as we have already,
        //    if (count <= this._readBuffer.Count) {
        //        // read what we have into the buffer and return
        //        Array.Copy(_readBuffer.Remove(count), 0, buffer, index, count);
        //        return count;
        //    }
        //    // if they want more than is in the buffer
        //    else {

        //        // think this needs to be in a task
        //        //// async, so spin up a new task to go and read on
        //        //return await Task<int>.Run(() => {

        //        this.DataReceived += ResetReadThread;

        //        // capture the count
        //        int bytesRead = _readBuffer.Count;

        //        // read what we can
        //        Array.Copy(_readBuffer.Remove(count), 0, buffer, index, count);

        //        // while we haven't read everything, and we haven't timed out
        //        while (bytesRead < count) {
        //            // set a wait handle to wait for the data received event
        //            // when it's reset, read some more
        //            readThreadResetEvent.WaitOne();

        //            // read read read
        //            bytesRead += _readBuffer.Count;
        //            int dataLength = _readBuffer.Count;
        //            Array.Copy(_readBuffer.Remove(dataLength), 0, buffer, bytesRead, dataLength);
        //        }

        //        // cleanup
        //        this.DataReceived += ResetReadThread;

        //        //});

        //    }

        //    void ResetReadThread(object sender, SerialDataReceivedEventArgs e)
        //    {
        //        readThreadResetEvent.Set();
        //    }

        //}


        /// <summary>
        /// Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
        /// </summary>
        /// <param name="buffer">The byte array to write the input to.</param>
        /// <param name="index">The offset in buffer at which to write the bytes.</param>
        /// <param name="count">The maximum number of bytes to read. Fewer bytes are read if count is greater than the number of bytes in the input buffer.</param>
        /// <returns>The number of bytes read.</returns>
        /// <exception cref="TimeoutException">No bytes were available to read.</exception>
        public int Read(byte[] buffer, int index, int count)
        {
            if (_readBuffer == null) return 0;

            // all the checks
            if (!IsOpen) { throw new InvalidOperationException("Cannot read from a closed port"); }
            if (buffer == null) { throw new ArgumentNullException(); }
            if (count > (buffer.Length - index)) { throw new ArgumentException("Count is larger than available buffer size"); }
            if (index < 0) { throw new ArgumentException("Invalid offset"); }
            if (count == 0) { return 0; }

            if (count > _readBuffer.Count) { throw new ArgumentException("Count cannot be larger than the available data."); }

            // read what we have into the buffer and return
            return _readBuffer.MoveItemsTo(buffer, index, count);
        }
    }
}
