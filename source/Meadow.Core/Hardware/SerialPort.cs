using System.IO;


//TODO - stub pulled from Microsoft.SPOT.Hardware
namespace Meadow.Hardware
{
   
    public enum Parity
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4
    }

    //TODO
    public enum StopBits
    {
        None = 0,
        One = 1,
        Two = 2,
        OnePointFive = 3
    }
    public enum Handshake
    {
        None = 0,
        RequestToSend = 6,
        XOnXOff = 24
    }

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
        public Handshake Handshake { get; set; }
        public bool IsOpen { get; }
        public Parity Parity { get; set; }
        public string PortName { get; }
        public StopBits StopBits { get; set; }

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
        public SerialPort(string portName, int baudRate, Parity parity) { }
        public SerialPort(string portName, int baudRate, Parity parity, int dataBits) { }
        public SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits) { }

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

    public class SerialErrorReceivedEventArgs
    {
        public SerialError EventType { get; }
    }

    public enum SerialError
    {
        TXFull = 0,
        RXOver = 1,
        Overrun = 2,
        RXParity = 3,
        Frame = 4
    }

    public class SerialDataReceivedEventArgs
    {
        public SerialData EventType { get; }
    }

    public enum SerialData
    {
        Chars = 0,
        Eof = 1
    }
}
