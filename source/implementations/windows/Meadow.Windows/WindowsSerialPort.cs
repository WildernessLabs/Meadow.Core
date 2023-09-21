using Meadow.Hardware;
using System;
using System.IO.Ports;
using System.Linq;

namespace Meadow;

public class WindowsSerialPort : ISerialPort, IDisposable
{
    public event Hardware.SerialDataReceivedEventHandler DataReceived = delegate { };
    public event EventHandler BufferOverrun = delegate { };

    private SerialPort _port;

    public int BytesToRead => _port.BytesToRead;
    public bool IsOpen => _port.IsOpen;
    public string PortName => _port.PortName;
    public int ReceiveBufferSize => _port.ReadBufferSize;

    public WindowsSerialPort(
        SerialPortName portName,
        int baudRate,
        int dataBits = 8,
        Hardware.Parity parity = Hardware.Parity.None,
        Hardware.StopBits stopBits = Hardware.StopBits.One,
        int readBufferSize = 4096)
    {
        var name = SerialPort.GetPortNames().FirstOrDefault(n => string.Compare(n, portName.SystemName, true) == 0 || string.Compare(n, portName.FriendlyName, true) == 0);

        if (name == null)
        {
            throw new ArgumentException($"Cannot find a serial port iwth a name matching '{portName.SystemName}' or '{portName.FriendlyName}'");
        }

        _port = new SerialPort(name, baudRate, (System.IO.Ports.Parity)parity, dataBits, (System.IO.Ports.StopBits)stopBits);
    }

    public int BaudRate
    {
        get => _port.BaudRate;
        set => _port.BaudRate = value;
    }

    public int DataBits
    {
        get => _port.DataBits;
        set => _port.DataBits = value;
    }

    public Hardware.Parity Parity
    {
        get => (Hardware.Parity)_port.Parity;
        set => _port.Parity = (System.IO.Ports.Parity)value;
    }

    public Hardware.StopBits StopBits
    {
        get => (Hardware.StopBits)_port.StopBits;
        set => _port.StopBits = (System.IO.Ports.StopBits)value;
    }

    public TimeSpan ReadTimeout
    {
        get => TimeSpan.FromMilliseconds(_port.ReadTimeout);
        set => _port.ReadTimeout = (int)value.TotalMilliseconds;
    }

    public TimeSpan WriteTimeout
    {
        get => TimeSpan.FromMilliseconds(_port.WriteTimeout);
        set => _port.WriteTimeout = (int)value.TotalMilliseconds;
    }

    public void ClearReceiveBuffer()
    {
        _port.DiscardOutBuffer();
    }

    public void Close()
    {
        _port.Close();
    }

    public void Dispose()
    {
        _port.Dispose();
    }

    public void Open()
    {
        _port.Open();
    }

    public int Peek()
    {
        throw new NotSupportedException();
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        return _port.Read(buffer, offset, count);
    }

    public byte[] ReadAll()
    {
        var buffer = new byte[_port.BytesToRead];
        _port.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public int ReadByte()
    {
        return _port.ReadByte();
    }

    public int Write(byte[] buffer)
    {
        _port.Write(buffer, 0, buffer.Length);
        return buffer.Length;
    }

    public int Write(byte[] buffer, int offset, int count)
    {
        _port.Write(buffer, offset, count);
        return count;
    }

}
