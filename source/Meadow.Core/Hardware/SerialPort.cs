using System.IO;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of serial (UART) communications.
    /// 
    /// NOTE: This class is not yet implemented. 
    /// </summary>
    public class SerialPort : Stream
    {
        public Stream BaseStream { get; }
        public int BaudRate { get; set; }
        public int BytesToRead { get; }
        public int BytesToWrite { get; }
        public int DataBits { get; set; }
        public FlowControlType Handshake { get; set; }
        public bool IsOpen { get; }
        public ParityType Parity { get; set; }
        public string PortName { get; }
        public NumberOfStopBits StopBits { get; set; }

        public override bool CanRead => throw new System.NotImplementedException();

        public override bool CanSeek => throw new System.NotImplementedException();

        public override bool CanWrite => throw new System.NotImplementedException();

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override int WriteTimeout { get; set; }

        public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);
        public delegate void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e);

        public event SerialDataReceivedEventHandler DataReceived;
        public event SerialErrorReceivedEventHandler ErrorReceived;

        public SerialPort(string portName) { }
        public SerialPort(string portName, int baudRate) { }
        public SerialPort(string portName, int baudRate, ParityType parity) { }
        public SerialPort(string portName, int baudRate, ParityType parity, int dataBits) { }
        public SerialPort(string portName, int baudRate, ParityType parity, int dataBits, NumberOfStopBits stopBits) { }

        public void DiscardInBuffer() { }
        public void DiscardOutBuffer() { }

        public void Open() { }
        
        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }

}
