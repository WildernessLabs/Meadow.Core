namespace Meadow.Hardware
{
    public interface ISerialPort
    {
        int BaudRate { get; set; }
        int BytesToRead { get; }
        int DataBits { get; }
        bool IsOpen { get; }
        Parity Parity { get; }
        string PortName { get; }
        int ReadBufferSize { get; }
        int ReadTimeout { get; set; }
        StopBits StopBits { get; }

        event SerialDataReceivedEventHandler DataReceived;

        void Close();
        void DiscardInBuffer();
        void Open();
        int Peek();
        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
        byte[] ReadToToken(byte token);
        string ToString();
        int Write(byte[] buffer);
        int Write(byte[] buffer, int offset, int count);
    }
}